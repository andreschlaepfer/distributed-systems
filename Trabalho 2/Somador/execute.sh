CSV_PATH=results.csv

echo "K,N=10^7,N=10^8,N=10^9" >> $CSV_PATH

gcc somador.c -o somador

for K in 1 2 4 8 16 32 64 128 256
  do
    echo "" >> $CSV_PATH
    echo -n $K >> $CSV_PATH
    for N in 10000000 100000000
      do 
        echo -n ,$(./somador $N $K) >> $CSV_PATH
    done
done

rm somador