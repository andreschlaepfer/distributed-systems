RESULTS_FOLDER=Resultados
LOG_FILE=$RESULTS_FOLDER/log.txt
RESULTS_FILE=$RESULTS_FOLDER/resultado.txt

CLIENT_FOLDER=Client
COORDINATOR_FOLDER=Coordinator

rm $RESULTS_FOLDER/resultado.txt
rm $RESULTS_FOLDER/coordinator.txt


# test_0
for i in 2; do
    cd $CLIENT_FOLDER && dotnet run $i 10 2 &
    wait

    mkdir -p $RESULTS_FOLDER/test_0/n=$i
    cp $RESULTS_FOLDER/resultado.txt $RESULTS_FOLDER/test_0/n=$i/resultado.txt
    rm $RESULTS_FOLDER/resultado.txt

    cp $RESULTS_FOLDER/log.txt $RESULTS_FOLDER/test_0/n=$i/log.txt
    rm $RESULTS_FOLDER/log.txt
done


# test_1
for i in 2 4 8 16 32; do
    cd $CLIENT_FOLDER && dotnet run $i 10 2 &
    wait

    mkdir -p $RESULTS_FOLDER/test_1/n=$i
    cp $RESULTS_FOLDER/resultado.txt $RESULTS_FOLDER/test_1/n=$i/resultado.txt
    rm $RESULTS_FOLDER/resultado.txt

    cp $RESULTS_FOLDER/log.txt $RESULTS_FOLDER/test_1/n=$i/log.txt
    rm $RESULTS_FOLDER/log.txt
done

# test_2
for i in 2 4 8 16 32 64; do
    cd $CLIENT_FOLDER && dotnet run $i 5 1 &
    wait

    mkdir -p $RESULTS_FOLDER/test_2/n=$i
    cp $RESULTS_FOLDER/resultado.txt $RESULTS_FOLDER/test_2/n=$i/resultado.txt
    rm $RESULTS_FOLDER/resultado.txt

    cp $RESULTS_FOLDER/log.txt $RESULTS_FOLDER/test_2/n=$i/log.txt
    rm $RESULTS_FOLDER/log.txt
done


# test_3
for i in 2 4 8 16 32 64 128; do
    cd $CLIENT_FOLDER && dotnet run $i 3 0 &
    wait

    mkdir -p $RESULTS_FOLDER/test_3/n=$i
    cp $RESULTS_FOLDER/resultado.txt $RESULTS_FOLDER/test_3/n=$i/resultado.txt
    rm $RESULTS_FOLDER/resultado.txt

    cp $RESULTS_FOLDER/log.txt $RESULTS_FOLDER/test_3/n=$i/log.txt
    rm $RESULTS_FOLDER/log.txt
done