CSV_PATH=results.csv

printf "Comb,N=1,N=2,N=4,N=16,N=32" >> $CSV_PATH

gcc producer_consumer.c -o producer_consumer

for K in "1 1" "1 2" "1 4" "1 8" "2 1" "4 1" "8 1"
  do
    printf "\n" >> $CSV_PATH
    printf "($K)" >> $CSV_PATH
    for N in 1, 2, 4, 16, 32
      do
        printf ",$(./producer_consumer $N $K)" >> $CSV_PATH
    done
done

rm producer_consumer