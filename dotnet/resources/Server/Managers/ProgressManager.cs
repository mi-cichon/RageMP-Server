using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    public class ProgressManager
    {

        List<int> expList = new List<int>()
        {
            0,  //1 lvl
            150,
            500,
            900,
            1400,
            2000,
            2800,
            4000,
            5500,
            7500,
            10000,
            12800,
            15600,
            18600,
            20000,
            24000,
            29000,
            35000,
            42000,
            50000 //20 lvl
        };

        List<string[]> jobs = new List<string[]>()
        {
            new string[]{ "warehouse", "Magazynier" },
            new string[]{ "lawnmowing", "Koszenie trawników" },
            new string[]{ "debriscleaner", "Zbieranie odpadów" },
            new string[]{ "hunter", "Myśliwy" },
            new string[]{ "forklifts", "Wózki widłowe" },
            new string[]{ "refinery", "Rafineria" },
            new string[]{ "diver", "Nurek" },
            new string[]{ "fisherman", "Wędkarstwo" },
            new string[]{ "gardener", "Ogrodnik" },
            new string[]{ "towtruck", "Lawety" }
        };

        public ProgressManager()
        {

        }

        public string[] GetPlayersProgressInfo(Player player)
        {
            List<string[]> jobLvls = new List<string[]>();
            foreach(string[] job in jobs)
            {
                jobLvls.Add(new string[] {job[1], GetJobLevelByExperience(player.GetSharedData<int>($"jp_{job[0]}")).ToString()});
            }

            return new string[] { JsonConvert.SerializeObject(jobLvls), JsonConvert.SerializeObject(jobBonuses) };
        }

        public string GetPlayersJobInfo(Player player)
        {
            List<string[]> jobsInfo = new List<string[]>();
            foreach(string[] job in jobs)
            {
                string jobName = job[0];
                string jobFullName = job[1];

                int exp = player.GetSharedData<int>($"jp_{jobName}");
                int nextLevelExp = GetNextJobLvlExperience(exp);
                int lvl = GetJobLevelByExperience(exp);
                int currentLevelExp = expList[lvl - 1];

                jobsInfo.Add(new string[] { jobFullName, lvl.ToString(), exp.ToString(), nextLevelExp.ToString(), currentLevelExp.ToString() });

            }
            return JsonConvert.SerializeObject(jobsInfo);
        }

        public int GetNextJobLvlExperience(int exp)
        {
            int lastLevel = 1;
            for (int i = 0; i < expList.Count; i++)
            {
                if (expList[i] > exp)
                {
                    break;
                }
                else
                {
                    lastLevel = i + 1;
                }
            }

            if(lastLevel == 20)
            {
                return 0;
            }
            else
            {
                return expList[lastLevel];
            }
        }

        public int GetJobLevelByExperience(int exp)
        {
            int lastLevel = 1;
            for (int i=0; i<expList.Count; i++)
            {
                if(expList[i] > exp)
                {
                    break;
                }
                else
                {
                    lastLevel = i + 1;
                }
            }
            return lastLevel;
        }

        public void SetPlayersJobBonuses(Player player)
        {
            foreach(JobBonus jobBonus in jobBonuses)
            {
                bool state = true;
                if(jobBonus.Requirements != null)
                {
                    foreach (int[] req in jobBonus.Requirements)
                    {
                        int jobExp = player.GetSharedData<int>($"jp_{jobs[req[0]][0]}");
                        if (GetJobLevelByExperience(jobExp) < req[1])
                        {
                            state = false;
                            break;
                        }
                    }
                }
                //Console.WriteLine($"jobBonus_{jobBonus.Id}: {state}");
                player.SetSharedData($"jobBonus_{jobBonus.Id}", state);
            }
        }

        List<JobBonus> jobBonuses = new List<JobBonus>()
        {
            new JobBonus(0, "main", "Magazynier", -1, new double[] {1.25, 1.25}, null, "Roznoszenie paczek na magazynie w Paleto"),
                new JobBonus(1, "side", "Bonus 20%", 0, new double[] {0, 1}, new int[][] { new int[]{0, 5}}, "Zwiększenie zarobków o 20%"),
                    new JobBonus(2, "side", "Bonus 40%", 1, new double[] {-1, 1}, new int[][] { new int[]{0, 12}}, "Zwiększenie zarobków o 40%"),
                        new JobBonus(3, "side", "Bonus 60%", 2, new double[] {0, 1}, new int[][] { new int[]{0, 20}}, "Zwiększenie zarobków o 60%"),
                new JobBonus(6, "side", "Sprint", 0, new double[] {1, 0}, new int[][] { new int[]{0, 20}}, "Bieganie z paczką"),
                new JobBonus(7, "main", "Wózki widłowe", 0, new double[] {1.5, 1.5}, new int[][] { new int[]{0, 15}}, "Operator wózka widłowego w dokach Los Santos"),
                    new JobBonus(8, "side", "Bonus 10%", 7, new double[] {-1, 1}, new int[][] { new int[]{4, 5}}, "Zwiększenie zarobków o 5%"),
                        new JobBonus(9, "side", "Bonus 15%", 8, new double[] {0, 1}, new int[][] { new int[]{4, 10}}, "Zwiększenie zarobków o 15%"),
                            new JobBonus(10, "side", "Bonus 25%", 9, new double[] {-1, 1}, new int[][] { new int[]{4, 20}}, "Zwiększenie zarobków o 25%"),
                    new JobBonus(11, "side", "Paczki+", 7, new double[] {1, 0}, new int[][] { new int[]{4, 20}}, "Możliwość transportu specjalnych paczek"),
                    new JobBonus(12, "main", "Rafineria", 7, new double[] {1.5, 1.5}, new int[][] { new int[]{4, 20}}, "Zaopatrywanie stacji benzynowych w ropę naftową"),
                        new JobBonus(13, "side", "Zlecenia II", 12, new double[] {-1, 1}, new int[][] { new int[]{5, 10}}, "Odblokowuje zlecenia poziomu drugiego"),
                            new JobBonus(14, "side", "Zlecenia III", 13, new double[] {-1, 1}, new int[][] { new int[]{5, 15}}, "Odblokowuje zlecenia poziomu trzeciego"),
                        new JobBonus(15, "side", "Cysterna 25%", 12, new double[] {1, -1}, new int[][] { new int[]{5, 10}}, "Zwiększa pojemność cysterny o 25%"),
                            new JobBonus(16, "side", "Cysterna 50%", 15, new double[] {1, 0}, new int[][] { new int[]{5, 15}}, "Zwiększa pojemność cysterny o 50%"),
                                new JobBonus(17, "side", "Cysterna 100%", 16, new double[] {1, 0}, new int[][] { new int[]{5, 20}}, "Zwiększa pojemność cysterny o 100%"),
                        new JobBonus(18, "side", "Pompa 10%", 12, new double[] {0, 1}, new int[][] { new int[]{5, 10}}, "Zwiększa wydajność pompy o 10%"),
                            new JobBonus(19, "side", "Pompa 25%", 18, new double[] {1, 1}, new int[][] { new int[]{5, 15}}, "Zwiększa wydajność pompy o 25%"),
                                new JobBonus(20, "side", "Pompa 40%", 19, new double[] {0, 1}, new int[][] { new int[]{5, 20}}, "Zwiększa wydajność pompy o 40%"),
                    new JobBonus(21, "main", "Lawety", 7, new double[] {1.5, -1.5}, new int[][] { new int[]{0, 20}, new int[] {4, 10}}, "Holowanie wyznaczonych wraków pojazdów"),
                        new JobBonus(22, "side", "Linka 25%", 21, new double[] {1, 1}, new int[][] { new int[]{9, 5}}, "Zwiększa długość linki holowniczej o 25%"),
                            new JobBonus(23, "side", "Linka 50%", 22, new double[] {1, 0}, new int[][] { new int[]{9, 15}}, "Zwiększa długość linki holowniczej o 50%"),
                        new JobBonus(24, "side", "Wyciągarka 25%", 21, new double[] {1, 0}, new int[][] { new int[]{9, 5}}, "Zwiększa wydajność wyciągarki o 25%"),
                            new JobBonus(25, "side", "Wyciągarka 50%", 24, new double[] {1, 0}, new int[][] { new int[]{9, 10}}, "Zwiększa wydajność wyciągarki o 50%"),
                                new JobBonus(26, "side", "Wyciągarka 75%", 25, new double[] {1, 0}, new int[][] { new int[]{9, 15}}, "Zwiększa wydajność wyciągarki o 75%"),
                        new JobBonus(27, "side", "Awans", 21, new double[] {1, -1}, new int[][] { new int[]{9, 10}}, "Uprawnienia na holowanie pojazdów graczy"),
                            new JobBonus(28, "side", "Hangar", 27, new double[] {1, 0}, new int[][] { new int[]{9, 20}}, "Uprawnienia na odstawianie holowanych pojazdów w hangarze w Sandy Shores"),


            new JobBonus(50, "main", "Zbieranie odpadów", -1, new double[] {-1.25, 1.25}, null, "Oczyszczanie plaży Vespucci ze śmieci"),
                new JobBonus(51, "side", "Szczęście I", 50, new double[] {0, 1}, new int[][]{ new int[] {2, 10 }}, "5% szans na znalezienie rzadkiego przedmiotu"),
                    new JobBonus(52, "side", "Szczęście II", 51, new double[] {0, 1}, new int[][]{ new int[] {2, 20 }}, "10% szans na znalezienie rzadkiego przedmiotu"),
                new JobBonus(53, "side", "Worek I", 50, new double[] {-1, -1}, new int[][]{ new int[] {2, 5 }}, "Worek o pojemności 60L"),
                    new JobBonus(54, "side", "Worek II", 53, new double[] {-1, 0}, new int[][]{ new int[] {2, 15 }}, "Worek o pojemności 90L"),
                        new JobBonus(55, "side", "Worek II", 54, new double[] {-1, -1}, new int[][]{ new int[] {2, 20 }}, "Worek o pojemności 120L"),
                new JobBonus(56, "side", "Bonus 5%", 50, new double[] {-1, 0}, new int[][]{ new int[] {2, 5 }}, "Zwiększenie zarobków o 5%"),
                    new JobBonus(57, "side", "Bonus 15%", 56, new double[] {-1, 0}, new int[][]{ new int[] {2, 10 }}, "Zwiększenie zarobków o 15%"),
                        new JobBonus(58, "side", "Bonus 30%", 57, new double[] {-1, 1}, new int[][]{ new int[] {2, 15 }}, "Zwiększenie zarobków o 30%"),
                            new JobBonus(59, "side", "Bonus 50%", 58, new double[] {-1, 0}, new int[][]{ new int[] {2, 20 }}, "Zwiększenie zarobków o 50%"),
                new JobBonus(60, "main", "Nurek", 50, new double[] {-1.5, 1.5}, new int[][]{ new int[] {2, 15 }}, "Szukanie przedmiotów w głębinach Alamo Sea"),
                    new JobBonus(61, "side", "Intuicja I", 60, new double[] {0, 1}, new int[][]{ new int[] {6, 10 }}, "Zwężenie obszaru poszukiwań o 25%"),
                        new JobBonus(62, "side", "Intuicja II", 61, new double[] {1, 1}, new int[][]{ new int[] {6, 20 }}, "Zwężenie obszaru poszukiwań o 50%"),
                    new JobBonus(63, "side", "Bonus 10%", 60, new double[] {-1, 0}, new int[][]{ new int[] {6, 5 }}, "Zwiększenie zarobków o 10%"),
                        new JobBonus(64, "side", "Bonus 15%", 63, new double[] {-1, 0}, new int[][]{ new int[] {6, 15 }}, "Zwiększenie zarobków o 15%"),
                            new JobBonus(65, "side", "Bonus 25%", 64, new double[] {-1, 1}, new int[][]{ new int[] {6, 20 }}, "Zwiększenie zarobków o 25%"),
                    new JobBonus(66, "main", "Wędkarstwo", 60, new double[] {-1.5, 1.5}, new int[][]{ new int[] {2, 20 }, new int[] {6, 15 }}, "Wędkowanie w zbiornikach wodnych w Los Santos i okolicach"),
                        new JobBonus(67, "side", "Spławik I", 66, new double[] {0, 1}, new int[][]{ new int[] {7, 10 }}, "Krótsze oczekiwanie na branie o 10%"),
                            new JobBonus(68, "side", "Spławik II", 67, new double[] {1, 1}, new int[][]{ new int[] {7, 20 }}, "Krótsze oczekiwanie na branie o 20%"),
                        new JobBonus(69, "side", "Oceany", 66, new double[] {-1, 0}, new int[][]{ new int[] {7, 10 }}, "Odblokowanie łowisk słonowodnych"),
                        new JobBonus(70, "side", "Wędka+", 66, new double[] {-1, 1}, new int[][]{ new int[] {7, 10 }}, "Możliwość zakupu drewnianej wędki u rybaka"),
                            new JobBonus(71, "side", "Wędka++", 70, new double[] {-1, 1}, new int[][]{ new int[] {7, 15 }}, "Możliwość zakupu plastikowej wędki u rybaka"),
                                new JobBonus(72, "side", "Wędka+++", 71, new double[] {-1, 0}, new int[][]{ new int[] {7, 20 }}, "Możliwość zakupu wędki z włókna węglowego u rybaka"),


            new JobBonus(100, "main", "Koszenie trawników", -1, new double[] {0, -1}, null, "Koszenie trawnika na polu golfowym na Rockford Hills"),
                new JobBonus(101, "side", "Kosz 25%", 100, new double[] {-1, 0}, new int[][]{ new int[] {1, 5 }}, "Pojemność kosza na trawę zwiększona o 25%"),
                    new JobBonus(102, "side", "Kosz 50%", 101, new double[] {-1, -1}, new int[][]{ new int[] {1, 10 }}, "Pojemność kosza na trawę zwiększona o 50%"),
                        new JobBonus(103, "side", "Kosz 100%", 102, new double[] {-1, 0}, new int[][]{ new int[] {1, 20 }}, "Pojemność kosza na trawę zwiększona o 100%"),
                new JobBonus(104, "side", "Bonus 5%", 100, new double[] {1, 0}, new int[][]{ new int[] {1, 5 }}, "Zwiększenie zarobków o 5%"),
                    new JobBonus(105, "side", "Bonus 15%", 104, new double[] {1, -1}, new int[][]{ new int[] {1, 10 }}, "Zwiększenie zarobków o 15%"),
                        new JobBonus(106, "side", "Bonus 30%", 105, new double[] {1, 0}, new int[][]{ new int[] {1, 15 }}, "Zwiększenie zarobków o 30%"),
                            new JobBonus(107, "side", "Bonus 50%", 106, new double[] {1, -1}, new int[][]{ new int[] {1, 20 }}, "Zwiększenie zarobków o 50%"),
                new JobBonus(108, "main", "Ogrodnik", 100, new double[] {0, -1.5}, new int[][]{ new int[] {1, 15 }}, "Zbieranie kwiatów na okolicznych łąkach"),
                    new JobBonus(109, "side", "Szczęście I", 108, new double[] {-1, 0}, new int[][] { new int[]{8, 5}}, "5% szans na zebranie dwóch roślin"),
                        new JobBonus(110, "side", "Szczęście II", 109, new double[] {-1, -1}, new int[][] { new int[]{8, 10}}, "10% szans na zebranie dwóch roślin"),
                            new JobBonus(111, "side", "Szczęście III", 110, new double[] {-1, 0}, new int[][] { new int[]{8, 20}}, "20% szans na zebranie dwóch roślin"),
                    new JobBonus(112, "side", "Sekator", 108, new double[] {-1, -1}, new int[][] { new int[]{8, 12}}, "Szybsze zbieranie roślin"),
                    //new JobBonus(113, "side", "Pojazd++", 108, new double[] {1, -1}, new int[][] { new int[]{8, 20}}, "Możliwość używania Bison jako pojazdu pracy"),
                    new JobBonus(114, "side", "Bonus 5%", 108, new double[] {1, 0}, new int[][] { new int[]{8, 5}}, "Zwiększenie zarobków o 5%"),
                        new JobBonus(115, "side", "Bonus 15%", 114, new double[] {1, -1}, new int[][] { new int[]{8, 10}}, "Zwiększenie zarobków o 15%"),
                            new JobBonus(116, "side", "Bonus 25%", 115, new double[] {1, 0}, new int[][] { new int[]{8, 20}}, "Zwiększenie zarobków o 25%"),
                    new JobBonus(117, "main", "Myśliwy", 108, new double[] {0, -1.5}, new int[][]{ new int[] {1, 20 }, new int[] {8, 15 }}, "Polowanie na zwierzynę w lasach przy Mount Chilliad"),
                        new JobBonus(118, "side", "Snajper", 117, new double[] {-1, -1}, new int[][] { new int[]{3, 10}}, "Polowanie przy użyciu karabinu snajperskiego"),
                        new JobBonus(119, "side", "Tropiciel", 117, new double[] {0, -1}, new int[][] { new int[]{3, 10}}, "Skuteczniejsze tropienie zwierzyny"),
                            new JobBonus(120, "side", "Nóż myśliwski", 119, new double[] {1, -1}, new int[][] { new int[]{3, 15}}, "Szybsze skórowanie zwierzyny"),
                        new JobBonus(121, "side", "Sokole oko I", 117, new double[] {1, -1}, new int[][] { new int[]{3, 5}}, "5% większa szansa na rzadsze zwierzę"),
                            new JobBonus(122, "side", "Sokole oko II", 121, new double[] {1, 0}, new int[][] { new int[]{3, 10}}, "7% większa szansa na rzadsze zwierzę"),
                                new JobBonus(123, "side", "Sokole oko III", 122, new double[] {1, 0}, new int[][] { new int[]{3, 15}}, "10% większa szansa na rzadsze zwierzę"),


        };              
    }

    public class JobBonus
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public double[] Offset { get; set; }
        public int[][] Requirements { get; set; }
        public string Description { get; set; }

        public JobBonus(int id, string type, string name, int parentID, double[] offset, int[][] requirements, string description)
        { 
            Id = id;
            Type = type;
            Name = name;
            ParentID = parentID;
            Offset = offset;
            Requirements = requirements;
            Description = description;
        }
    }
}
