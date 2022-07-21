FOLDER_0=Resultados/test_0
FOLDER_1=Resultados/test_1
FOLDER_2=Resultados/test_2
FOLDER_3=Resultados/test_3

VALIDATOR=Resultados/log_validator.py

for subfolder in $FOLDER_0/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done

for subfolder in $FOLDER_1/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done

for subfolder in $FOLDER_2/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done

for subfolder in $FOLDER_3/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done
