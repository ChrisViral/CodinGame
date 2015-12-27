(ns Solution
    (:gen-class))

(defn -main [& args]
    ;Set input variables
    (let [L (read)                          ;Size of the ride
          C (read)                          ;Amount of rides in the day
          N (read)                          ;Amount of groups in queue
          queue (vec (repeatedly N read))]  ;Queue vector
         
    ;Gets group next queue index and profit
    ;param{Int} start: Starting queue index
    ;returns{Map}: The next index and profit in a map
    (defn get-group [start]
        (loop [total 0
               i start
               loops 0]
           ;While in queue and that the ride isn't full
            (if (and (< loops N) (<= (+ total (queue i)) L))
                (recur (+ total (queue i))
                       (mod (inc i) N)
                       (inc loops))
                ;Else return new map
                {:i i :total total})))       
    
    ;If there is no info map for this index, creates one, else returns the general map
    ;param{Map} info: Map of all info maps for the queue
    ;param{Int} i: Integer index of the group in the queue
    ;return{Map}: The map with the info for the new group added if necessary
    (defn get-info [info i]
        (let [si (str i)]
            (if (contains? info si)
                info
                (assoc info si (get-group i)))))
        
    ;Gets the profit for a day
    ;returns{Int}: The profit for the starting parameters
    (defn get-profit []
        (loop [profit 0
               passes 0
               i 0
               dict {}]
           ;While the ride continues
            (if (< passes C)
                (let [info (get-info dict i)
                      g (info (str i))]
                    ;Add the profit for this index and skip to next index
                    (recur (+ profit (g :total))
                           (inc passes)
                           (g :i)
                           info))
                profit)))
    
    ;Prints answer
    (println (get-profit))))