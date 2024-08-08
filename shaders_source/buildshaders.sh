cd ../shaders_source
for f in *.fx; do
  wine fxc2.exe /T fx_2_0 $f /Fo ./../bin/content/shaders/${i%%.*}.xnb
done


