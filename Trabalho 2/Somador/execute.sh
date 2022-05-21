CSV_PATH=results.csv

echo "K,N=10^7,N=10^8,N=10^9" >> $CSV_PATH

gcc somador.c -o somador

for K in 1 2 4 8 16 32 64 128 256
  do
    VALUES=$K
    for N in 100000000
      do 
        VALUES=$VALUES,$(./somador $N $K)
    done
    echo $VALUES
    echo $VALUES >> $CSV_PATH
done

rm somador