#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIGURATION="${1:-Debug}"
CPU_COUNT="$(sysctl -n hw.ncpu)"
CMAKE_POLICY_VERSION_MINIMUM="${CMAKE_POLICY_VERSION_MINIMUM:-3.5}"

require_command() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Missing required command: $1" >&2
    exit 1
  fi
}

copy_sdl2_dylib() {
  local sdl2_dylib=""
  local brew_prefix=""

  if command -v brew >/dev/null 2>&1; then
    brew_prefix="$(brew --prefix sdl2 2>/dev/null || true)"
    if [[ -n "$brew_prefix" && -f "$brew_prefix/lib/libSDL2-2.0.0.dylib" ]]; then
      sdl2_dylib="$brew_prefix/lib/libSDL2-2.0.0.dylib"
    fi
  fi

  if [[ -z "$sdl2_dylib" && -f "/opt/homebrew/lib/libSDL2-2.0.0.dylib" ]]; then
    sdl2_dylib="/opt/homebrew/lib/libSDL2-2.0.0.dylib"
  fi

  if [[ -z "$sdl2_dylib" && -f "/usr/local/lib/libSDL2-2.0.0.dylib" ]]; then
    sdl2_dylib="/usr/local/lib/libSDL2-2.0.0.dylib"
  fi

  if [[ -z "$sdl2_dylib" ]]; then
    echo "Could not find libSDL2-2.0.0.dylib. Install sdl2 first (brew install sdl2)." >&2
    exit 1
  fi

  cp -f "$sdl2_dylib" "$ROOT_DIR/bin/libSDL2-2.0.0.dylib"
}

copy_libgdiplus_dylib() {
  local gdiplus_dylib=""
  local brew_prefix=""

  if command -v brew >/dev/null 2>&1; then
    brew_prefix="$(brew --prefix mono-libgdiplus 2>/dev/null || true)"
    if [[ -n "$brew_prefix" && -f "$brew_prefix/lib/libgdiplus.dylib" ]]; then
      gdiplus_dylib="$brew_prefix/lib/libgdiplus.dylib"
    fi
  fi

  if [[ -z "$gdiplus_dylib" && -f "/opt/homebrew/lib/libgdiplus.dylib" ]]; then
    gdiplus_dylib="/opt/homebrew/lib/libgdiplus.dylib"
  fi

  if [[ -z "$gdiplus_dylib" && -f "/usr/local/lib/libgdiplus.dylib" ]]; then
    gdiplus_dylib="/usr/local/lib/libgdiplus.dylib"
  fi

  if [[ -z "$gdiplus_dylib" ]]; then
    echo "Could not find libgdiplus.dylib. Install mono-libgdiplus (brew install mono-libgdiplus)." >&2
    exit 1
  fi

  cp -f "$gdiplus_dylib" "$ROOT_DIR/bin/libgdiplus.dylib"
}

require_command nuget
require_command xbuild
require_command cmake
require_command make
require_command mono

echo "[1/4] Restoring NuGet packages"
nuget restore "$ROOT_DIR/DuckGame.sln"

echo "[2/4] Building native macOS libraries"
cmake -S "$ROOT_DIR/FNA/lib/FAudio" -B "$ROOT_DIR/FNA/lib/FAudio/build" -DCMAKE_BUILD_TYPE=Release -DCMAKE_POLICY_VERSION_MINIMUM="$CMAKE_POLICY_VERSION_MINIMUM"
cmake --build "$ROOT_DIR/FNA/lib/FAudio/build" --config Release --parallel "$CPU_COUNT"

cmake -S "$ROOT_DIR/FNA/lib/FNA3D" -B "$ROOT_DIR/FNA/lib/FNA3D/build" -DCMAKE_BUILD_TYPE=Release -DCMAKE_POLICY_VERSION_MINIMUM="$CMAKE_POLICY_VERSION_MINIMUM"
cmake --build "$ROOT_DIR/FNA/lib/FNA3D/build" --config Release --parallel "$CPU_COUNT"

make -C "$ROOT_DIR/FNA/lib/Theorafile"

echo "[3/4] Building DuckGame ($CONFIGURATION) with Mono"
xbuild \
  /p:Configuration="$CONFIGURATION" \
  /p:SolutionDir="$ROOT_DIR/" \
  "$ROOT_DIR/DuckGame/DuckGame.csproj"

echo "[4/4] Copying runtime macOS libraries"
mkdir -p "$ROOT_DIR/bin"
cp -f "$ROOT_DIR/FNA/lib/FAudio/build/libFAudio.0.dylib" "$ROOT_DIR/bin/"
cp -f "$ROOT_DIR/FNA/lib/FNA3D/build/libFNA3D.0.dylib" "$ROOT_DIR/bin/"
cp -f "$ROOT_DIR/FNA/lib/Theorafile/libtheorafile.dylib" "$ROOT_DIR/bin/"
copy_sdl2_dylib
copy_libgdiplus_dylib
cp -f "$ROOT_DIR/deps/steam_api.bundle/Contents/MacOS/libsteam_api.dylib" "$ROOT_DIR/bin/libsteam_api.dylib"

echo "Build complete."
echo "Run with: cd \"$ROOT_DIR/bin\" && ./DuckGame.sh"
