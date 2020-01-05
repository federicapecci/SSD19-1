using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PyGAP2019
{
    class GAPclass
    {
        public int n;  // numero clienti
        public int m;  // numero magazzini
        public double[,] c;  // costi assegnamento
        public int[] req;    // richieste clienti
        public int[] cap;    // capacità magazzini

        public int[] sol, solbest;    // per ogni cliente, il suo magazzino 
        public double zub, zlb;

        const double EPS = 0.0001;
        System.Random rnd = new Random(550);

        public GAPclass()
        {
            zub = double.MaxValue;  //costo massimo di una soluzione
            zlb = double.MinValue;  //costo minimo di una soluzione 
        }

        //per ogni customer stabilisco qual è il magazzino più vicino ad esso (in termini di distanza)
        //e verifico qual è il primo magazzino disponibile che ha la capicità che serve al cliente
        // sol [cust0:idMag2, cust1:idMag1]
        public double SimpleContruct()
        {
            int i, ii, j;
            int[] capleft = new int[cap.Length];
            int[] ind = new int[m];
            //dist 
            double[] dist = new double[m];
            //cap = capacità di ogni magazzino
            Array.Copy(cap, capleft, cap.Length);
            zub = 0; 

            for(j=0; j<n; j++)
            {
                for (i = 0; i < m; i++)
                {
                    dist[i] = c[i, j];
                    ind[i] = i;
                }
                Array.Sort(dist, ind); //lasciare ordinato
                ii = 0;

                while (ii < m)
                {
                    i = ind[ii];
                    if (capleft[i] >= req[j]) // se la capacità richiesta è maggiore o = di quella richiesta dal cliente
                    {
                        sol[j] = i; // i = id del magazzino che assegno al customer j 
                        capleft[i] -= req[j]; //aggiorno la capacità rimasta
                        zub += c[i, j]; //aggiorno il costo della soluzione
                        //Trace.WriteLine($"[SimpleConstruct] Client {j} server {i}.");
                        break;
                    }
                    ii++;
                }
                if (ii == m) // se non trovo nessun magazzino disponibile per un customer
                {
                    Trace.WriteLine("[SimpleConstruct] Ahi Ahi.");
                }
            }          
            return zub;
        }

        //partendo dalla soluzione di simplecontruct 
        //controllo se la soluzione può essere ottimizzata
        //cercando se possibile di assegnare il customer ad un magazzino ad
        //esso più vicino e che abbia la capacità che il cliente richiede
        //***risultato rimane inviariato perché in simple construct ho già ordinato (per ogni cliente)
        //l'array delle distanze 
        //Opt10 -> si blocca nella ricerca degli ottimi locali
        public double opt10(double[,] c)
        {
            double z = 0;
            int i, isol, j;
            //capleft. capacità = numero dei magazzini
            int[] capres = new int[cap.Length]; 

            Array.Copy(cap, capres, cap.Length);
            //per ogni customer
            for (j = 0; j < n; j++)
            {
                //aggiorno la capacità del magazzino a cui il customer si rifornisce
                capres[sol[j]] -= req[j];
                z += c[sol[j], j];
            }

        l0: for (j = 0; j < n; j++)
            {
                isol = sol[j];
                for (i = 0; i < m; i++)
                {
                    if (i == isol) continue;
                    if (c[i, j] < c[isol, j] && capres[i] >= req[j])
                    {
                        sol[j] = i; //aggiorno l'id del magazzino per un certo customer
                        //riaggiusto le capacità dei magazzini in base ai magazzi ai quali il customer fa riferimento
                        capres[i] -= req[j];
                        capres[isol] += req[j]; 
                        //aggiorno il costo della soluzione
                        z -= (c[isol, j] - c[i, j]);
                        if (z < zub)
                        {
                            zub = z;
                            Trace.WriteLine("[1-0 opt] new zub " + zub);
                        }
                        goto l0;
                    }
                }
            }
            double zcheck = 0;
            for (j = 0; j < n; j++)
                zcheck += c[sol[j], j];
            if (Math.Abs(zcheck - z) > EPS)
                System.Windows.Forms.MessageBox.Show("[1.0opt]");
            return z;
        }

        // Tabu search
        public double TabuSearch(int Ttenure, int maxiter)
        {
            int i, isol, j, imax, jmax, iter;
            double z, DeltaMax;
            int[] capres = new int[cap.Length];
            int[,] TL = new Int32[m, n];
            /*
            1.	Generate an initial feasible solution S, (opt10)
	            set S* = S and initialize TL=nil.
            2.	Find S' \in N(S), such that (N -> neighborhood)
                //tra le soluzioni vicine prendo quella che ha il costo minore nel vicinato e che non è nella taboo list
	            z(S')=min {z(S^), foreach S^\in N(S), S^\notin TL}. 
                //la soluzione scelta finisce dentro la taboo list
            3.	S=S', TL=TL U {S}, if (z(S*) > z(S)) set S* = S. 
            4.	If not(end condition) go to step 2.
            */

            Array.Copy(cap, capres, cap.Length);
            //in base alla soluzione di opt10, vado a ricalcolare l'array con le capacità residue per ogni magazzino
            for (j = 0; j < n; j++)
                capres[sol[j]] -= req[j];

            //salvo in variabile il costo di opt10
            z = zub;
            iter = 0;
            //inializzo tutta la matrice della taboo list con -infinito
            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                    TL[i, j] = int.MinValue;

            Trace.WriteLine("Starting tabu search");
        start: DeltaMax = imax = jmax = int.MinValue;
            iter++;

            //FOR ANNIDATI -> determino la soluzione col costo minore nel vicinato
            //cerco di massimizzare la distanza tra la soluzione trovata con  GAP.opt10(GAP.c) e GAP.tabusearch
            //per ridurre il costo 
            //deltamax utilizzato per scegliere la nuova combinazione (customer - negozio) che 
            //abbassa maggiormente il costo
            //TENURE = per quanti cicli una soluzione rimane nella taboo list
            for (j = 0; j < n; j++)
            {
                isol = sol[j];
                for (i = 0; i < m; i++)
                {
                    if (i == isol) continue;
                    if ((c[isol, j] - c[i, j]) > DeltaMax && capres[i] >= req[j] && (TL[i, j] + Ttenure) < iter)
                    {
                        imax = i;
                        jmax = j;
                        DeltaMax = c[isol, j] - c[i, j];
                    }
                }
            }

            isol = sol[jmax];
            sol[jmax] = imax;
            // a seconda del magazzzino a cui associo il customer, rissitemo le capacità
            capres[imax] -= req[jmax];
            capres[isol] += req[jmax];
            z -= DeltaMax;
            if (z < zub)
                zub = z;
            TL[imax, jmax] = iter;
            Trace.WriteLine("[Tabu Search]  z " + z + " ite " + iter + " deltamax " + DeltaMax);

            //guardo se ho fatto tutte le iterazioni
            if (iter < maxiter)          // end condition
                goto start;
            else
                Trace.WriteLine("Tabu search: fine");

            double zcheck = 0;
            for (j = 0; j < n; j++)
                zcheck += c[sol[j], j];
            if (Math.Abs(zcheck - z) > EPS)
                System.Windows.Forms.MessageBox.Show("[tabu search]");
            //zub 
            return zub;
        }
    }
}
