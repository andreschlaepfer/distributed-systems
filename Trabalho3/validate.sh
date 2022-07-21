FOLDER_0=Resultados/test_0
FOLDER_1=Resultados/test_1
FOLDER_2=Resultados/test_2
FOLDER_3=Resultados/test_3

VALIDATOR=Resultados/log_validator.py
RES_VALIDATOR=Resultados/resultado_validator.py

# test_0
for subfolder in $FOLDER_0/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done

# test_1
for subfolder in $FOLDER_1/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done

# test_2
for subfolder in $FOLDER_2/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done

# test_3
for subfolder in $FOLDER_3/*; do
  python3 $VALIDATOR $(readlink -f  $subfolder)/log.txt
done


# test_0
for n in 2; do
  python3 $RES_VALIDATOR $(readlink -f  Resultados/test_0/n=$n)/resultado.txt $n 10
done

# test_1
for n in 2 4 8 16 32; do
  python3 $RES_VALIDATOR $(readlink -f  Resultados/test_1/n=$n)/resultado.txt $n 10
done

# test_2
for n in 2 4 8 16 32 64; do
  python3 $RES_VALIDATOR $(readlink -f  Resultados/test_2/n=$n)/resultado.txt $n 5
done

# test_3
for n in 2 4 8 16 32 64 128; do
  python3 $RES_VALIDATOR $(readlink -f  Resultados/test_3/n=$n)/resultado.txt $n 3
done