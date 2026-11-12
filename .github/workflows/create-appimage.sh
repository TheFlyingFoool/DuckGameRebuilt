set -euo pipefail

ROOT=$(pwd)
DGDIR="$ROOT/bin"
APPDIR="$ROOT/DuckGameRebuilt.AppDir"

rm -rf "$APPDIR"
mkdir -p "$APPDIR/usr/" "$APPDIR/usr/share/metainfo" \
         "$APPDIR/usr/share/icons/hicolor/256x256/apps" \
         "$APPDIR/usr/mono" "$APPDIR/etc"

cp -r "$DGDIR" "$APPDIR/usr/"

MONO="$APPDIR/usr/mono"
mkdir -p "$MONO/bin" "$MONO/lib" "$MONO/etc" "$MONO/lib/mono"

cp -L "$SDL_PREFIX/lib/libSDL3.so.0" "$APPDIR/usr/mono/lib/"
ln -sfn libSDL3.so.0 "$APPDIR/usr/mono/lib/libSDL3.so"

cp -L "$(readlink -f "$(command -v mono-sgen)")" "$MONO/bin/mono-sgen"

find_lib() {
  if [ -e "/usr/lib/$1" ]; then echo "/usr/lib/$1"
  elif [ -e "/usr/lib/x86_64-linux-gnu/$1" ]; then echo "/usr/lib/x86_64-linux-gnu/$1"
  else return 1; fi
}
for name in libmono-native.so libmono-native.so.0 libmono-btls-shared.so \
            libMonoPosixHelper.so libMonoSupportW.so libgdiplus.so.0; do
  src="$(find_lib "$name")" || { echo "ERROR: $name not found in /usr/lib[/x86_64-linux-gnu]" >&2; exit 1; }
  cp -L "$src" "$MONO/lib/$name"
done
for name in libgdiplus.so; do
  if src="$(find_lib "$name")"; then cp -L "$src" "$MONO/lib/$name"; fi
done

skip_core() {
  case "$1" in
    libc.so*|libm.so*|libdl.so*|libpthread.so*|librt.so*|ld-linux*) return 0 ;;
    *) return 1 ;;
  esac
}
for pass in 1 2 3 4 5; do
  copied=0
  for f in "$MONO"/lib/*.so*; do
    for src in $(ldd "$f" 2>/dev/null | awk '/=> \//{print $3}'); do
      name="$(basename "$src")"
      if skip_core "$name" || [ -e "$MONO/lib/$name" ]; then continue; fi
      cp -L "$src" "$MONO/lib/$name"
      copied=1
    done
  done
  if [ "$copied" = 0 ]; then break; fi
done

cp -a /usr/lib/mono/4.5 "$MONO/lib/mono/4.5"
cp -a /usr/lib/mono/gac "$MONO/lib/mono/gac"
cp -a /etc/mono "$MONO/etc/mono"

cat > "$APPDIR/AppRun" <<'EOF'
#!/usr/bin/env bash

APPDIR="${APPDIR:-}"
if [ -z "$APPDIR" ]; then APPDIR="$(dirname "$(readlink -f "$0")")"; fi
GAME="$APPDIR/usr/bin"
MONO="$APPDIR/usr/mono"

# Writable log location, the AppImage payload is read-only when mounted.
LOGDIR="${XDG_DATA_HOME:-$HOME/.local/share}/DuckGameRebuilt"
mkdir -p "$LOGDIR" 2>/dev/null || LOGDIR="$(mktemp -d)"

cd "$GAME" || exit 1

# Bundled Mono runtime (default) no system Mono needed
if [ -x "$MONO/bin/mono-sgen" ]; then
  echo "AppRun: Using bundled Mono at $MONO" >&2
  export MONO_GAC_PREFIX="$MONO"
  export MONO_CFG_DIR="$MONO/etc"
  export MONO_PATH="$MONO/lib/mono/4.5"
  export LD_LIBRARY_PATH="$MONO/lib:$GAME${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"

  "$MONO/bin/mono-sgen" ./DuckGame.exe "$@" 2>&1 | tee "$LOGDIR/outputlog.txt"
  exit "${PIPESTATUS[0]}"
fi

# System Mono fallback.
if command -v mono >/dev/null 2>&1; then
  echo "AppRun: bundled Mono failed, falling back to system mono." >&2
  export LD_LIBRARY_PATH="$GAME${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
  mono ./DuckGame.exe "$@" 2>&1 | tee "$LOGDIR/outputlog.txt"
  exit "${PIPESTATUS[0]}"
fi

echo "Failed to launch. Please install mono according to your distribution."
EOF
chmod +x "$APPDIR/AppRun"

cat > "$APPDIR/DuckGame.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=Duck Game Rebuilt
GenericName=Duck Game client
Comment=Duck Game decompiled & rebuilt with added features
Exec=AppRun
Icon=duckgamerebuilt
Terminal=false
Categories=Game;
EOF

cp "./icon.png" "$APPDIR/duckgamerebuilt.png"
cp "./icon.png" "$APPDIR/usr/share/icons/hicolor/256x256/apps/duckgamerebuilt.png"

chmod a+rX "$APPDIR" -R
chmod a+rx "$APPDIR/AppRun"

curl -fSL --retry 3 -o ./appimagetool.AppImage \
    "https://github.com/AppImage/appimagetool/releases/download/continuous/appimagetool-x86_64.AppImage"
chmod +x ./appimagetool.AppImage

ARCH=x86_64 "./appimagetool.AppImage" \
    --updateinformation "gh-releases-zsync|TheFlyingFoool|DuckGameRebuilt|latest|*.AppImage.zsync" \
    "$APPDIR" ./DuckGameRebuilt.AppImage
