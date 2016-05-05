read N              #Amount of horses
declare -a horses   #Array to store the horses

#Reading all values
for (( i=0; i<N; i++ )); do
    read h
    horses[i]=$h
done

#Sorting
horses=($(for i in "${horses[@]}"; do echo $i; done | sort -n))

smallest=$((${horses[1]} - ${horses[0]}))   #Set smallest diff to the diff of the two first horses
current=${horses[1]}                        #Current is second horse

#Loop through the remaining horses
for next in "${horses[@]:2}"; do
    diff=$(($next - $current))
    if [ $diff -lt $smallest ]; then
        smallest=$diff
    fi
    current=$next
done

#Write smallest
echo $smallest