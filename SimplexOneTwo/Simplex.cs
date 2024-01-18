using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexOneTwo
{
    internal class Simplex
    {
        List<double> funcHelp;
        List<List<double>> restr;
        List<int> basis;
        string solve,ans;
        bool canonical;
        public Simplex(int n, int m, string to, double[,] data, List<string> eq, List<double> function)
        {
            
            funcHelp = new List<double>();
            basis = new List<int>();
            restr = new List<List<double>>();

            for (int i = 0; i < m; i++)
            {
                restr.Add(new List<double>());
            }


            for(int i= 0; i < m; i++)
                for(int j = 0; j < n;j++)
                    restr[i].Add(data[i,j]);

            ToStdView(to, eq,function);

            canonical = IsCanonical(eq);
            if (!canonical)
            {
                AddFictionalVars(function);
            }

            InitBasis(function);


            //решать
            solve = "";
            int t = 0;

            solve += ($"{((canonical) ? "Одноэтапный" : "Двухэтапный")} симплекс метод\r\n");

            if (!canonical)
            {
                solve += (SimplexTable(function));
                while ((t = IterFirstPhase(function)) == 0)
                {
                    solve += ("\r\n--------------------\r\n");
                    solve += (SimplexTable(function));
                }
                solve += ("\r\n--------------------\r\n");
                solve += (SimplexTable(function));
                
                switch (t)
                {
                    case 1: solve += ("\r\nКонец Первого этапа\r\n"); break;
                    case 2:
                        {
                            solve += ("\r\nОграничения противоречивы\r\n");
                            return;
                        }; break;
                }
            }


            solve += (SimplexTable(function));
            while ((t = Iter(function)) == 0)
            {
                solve += ("\r\n--------------------\r\n");
                solve += (SimplexTable(function));
               
            }
            switch (t)
            {
                case 2: solve += ("\r\nF min = - бесконечность"); break;
            }
            ans = Answer(to, function);
            return;
        }
        public string GetSolveString() => solve;
        public string GetAnsString() => ans;
        private void InitBasis(List<double> func)
        {
            List<double> prebasis = new List<double>();
            HashSet<string> inds = new HashSet<string>();
            for (int i = 0; i < restr[0].Count; i++)
            {
                List<double> ind = new List<double>();
                double sum = 0;
                for (int j = 0; j < restr.Count; j++)
                {
                    sum += restr[j][i];
                    ind.Add(restr[j][i]);
                }
                if (sum == 1 && (ToHashSet(ind)).Count == 2 && (ToHashSet(ind)).Contains(0) && (ToHashSet(ind)).Contains(1))
                {
                    prebasis.Add(i);
                }
            }

            for (int i = 0; i < restr.Count; i++)
            {
                int ind = -1;
                for (int j = 0; j < restr[0].Count; j++)
                    if (restr[i][j] == 1 && prebasis.Contains(j)) ind = j;
                basis.Add(ind);
            }

            for (int i = 0; i < func.Count; i++)
            {
                if (basis.Contains(i) && func[i] != 0)
                {
                    int ind = -1;
                    for (int j = 0; j < restr.Count; j++)
                        if (restr[j][i] == 1) ind = j;
                    double coef = func[i] / restr[ind][i];
                    for (int j = 0; j < func.Count; j++)
                    {
                        func[j] -= coef * restr[ind][j];
                    }
                }
            }

            return;
        }
        private void AddFictionalVars(List<double> func)
        {
            while (funcHelp.Count <= restr[0].Count - 1)
                funcHelp.Add(0);

            for (int i = 0; i < restr.Count; i++)
            {
                for (int j = 0; j < restr[i].Count; j++)
                    funcHelp[j] -= restr[i][j];
            }

            for (int a = 0; a < restr.Count; a++)
            {
                for (int z = 0; z < restr.Count; z++)
                {
                    restr[z].Insert(restr[z].Count - 1, 0);
                }
                func.Insert(func.Count - 1, 0);
                funcHelp.Insert(funcHelp.Count - 1, 0);
            }
        }
        private void ToStdView(string to, List<string> eq, List<double> func)
        {
            if (to.Equals("Max"))
            {
                for (int i = 0; i < func.Count; i++)
                    func[i] *= -1;
            }
            int added = 0;
            for (int z = 0; z < eq.Count; z++)
            {
                switch (eq[z])
                {
                    case "=": continue;
                    case ">=":
                        {
                            for (int i = 0; i < restr.Count; i++)
                                restr[i].Insert(restr[i].Count - 1, 0);

                            restr[added][restr[added].Count - 2] = -1;
                            added++;
                            eq[z] = "=";
                        }; break;
                    case "<=":
                        {
                            for (int i = 0; i < restr.Count; i++)
                                restr[i].Insert(restr[i].Count - 1, 0);

                            restr[added][restr[added].Count - 2] = 1;
                            added++;
                            eq[z] = "=";
                        }; break;
                }
            }

            for (int z = 0; z < restr.Count; z++)
            {
                if (restr[z][restr[z].Count - 1] < 0)
                {
                    for (int g = 0; g < restr[z].Count; g++)
                        restr[z][g] *= -1;
                }
            }

            while (func.Count <= restr[0].Count - 1)
                func.Add(0);

            return;
        }
        private string SimplexTable(List<double> func)
        {
            string ans = "   | ";

            for (int i = 0; i < restr[0].Count - 1; i++)
            {
                ans += $"{"x" + (i + 1).ToString(),4}";
            }
            ans += "| знач\r\n";


            for (int i = 0; i < restr.Count; i++)
            {
                ans += $"x{basis[i] + 1} |";
                for (int j = 0; j < restr[i].Count - 1; j++)
                    ans += $"{restr[i][j],4}";
                ans += $" | {restr[i][restr[i].Count - 1]}\r\n";
            }
            ans += " f |";
            for (int i = 0; i < func.Count - 1; i++)
                ans += $"{func[i],4}";
            ans += $" | {func[func.Count - 1]}\r\n";

            if (!canonical)
            {
                ans += " f1|";
                for (int i = 0; i < funcHelp.Count - 1; i++)
                    ans += $"{funcHelp[i],4}";
                ans += $" | {funcHelp[funcHelp.Count - 1]}";
            }
            return ans;
        }

        private HashSet<double> ToHashSet(List<double> list)
        {
            return new HashSet<double>(list);
        }

        public bool IsCanonical(List<string> eq)
        {
            HashSet<string> inds = new HashSet<string>();
            for (int i = 0; i < restr[0].Count; i++)
            {
                List<double> ind = new List<double>();
                double sum = 0;
                for (int j = 0; j < restr.Count; j++)
                {
                    sum += restr[j][i];
                    ind.Add(restr[j][i]);
                }
                if (sum == 1 && (ToHashSet(ind)).Count == 2 && (ToHashSet(ind)).Contains(0) && (ToHashSet(ind)).Contains(1))
                {
                    string add = "";
                    for (int j = 0; j < restr.Count; j++)
                    {
                        add += restr[j][i].ToString();
                    }
                    inds.Add(add);
                }
            }
            return inds.Count == eq.Count;
        }
        private int IterFirstPhase(List<double> func)
        {
            // 0 - нужно ещё
            // 1 - конец
            // 2 - все плохо

            int ind_col = -1;
            double max = -1;
            for (int i = 0; i < funcHelp.Count - 1; i++)
                if (funcHelp[i] < 0 && Math.Abs(funcHelp[i]) > max)
                {
                    max = Math.Abs(funcHelp[i]);
                    ind_col = i;
                }

            if (ind_col == -1)
                return 2;

            int ind_row = -1;
            double min = Double.MaxValue / 3;
            for (int i = 0; i < restr.Count; i++)
            {
                if (restr[i][ind_col] <= 0)
                    continue;
                if (min > (restr[i][restr[i].Count - 1] / restr[i][ind_col]))
                {
                    min = restr[i][restr[i].Count - 1] / restr[i][ind_col];
                    ind_row = i;
                }
            }

            if (ind_row == -1)
                return 2;

            basis[ind_row] = ind_col;

            for (int i = 0; i < restr[ind_row].Count; i++)
                if (i != ind_col) restr[ind_row][i] /= restr[ind_row][ind_col];
            restr[ind_row][ind_col] = 1;

            for (int i = 0; i < restr.Count; i++)
            {
                if (i == ind_row) continue;
                double coef = -restr[i][ind_col];
                for (int j = 0; j < restr[i].Count; j++)
                    restr[i][j] += restr[ind_row][j] * coef;
            }

            double c = (-funcHelp[ind_col]);
            for (int i = 0; i < funcHelp.Count; i++)
            {
                double t = funcHelp[i] + restr[ind_row][i] * c;
                funcHelp[i] = t;
            }

            c = (-func[ind_col]);
            for (int i = 0; i < func.Count; i++)
            {
                double t = func[i] + restr[ind_row][i] * c;
                func[i] = t;
            }

            bool allplus = true;
            bool allzero = true;
            for (int i = 0; i < funcHelp.Count; i++)
            {
                if (funcHelp[i] < 0) allplus = false;
                if (Math.Abs(funcHelp[i]) >= 1e-6)
                    allzero = false;
            }
            if (allzero)
                return 1;
            if (allplus)
                return 2;
            return 0;
        }
        private int Iter(List<double> func)
        {
            int ind_col = -1;
            double max = -1;
            for (int i = 0; i < func.Count - 1; i++)
                if (func[i] < 0 && Math.Abs(func[i]) > max)
                {
                    max = Math.Abs(func[i]);
                    ind_col = i;
                }

            if (ind_col == -1)
                return 1;

            int ind_row = -1;
            double min = Double.MaxValue / 3;
            for (int i = 0; i < restr.Count; i++)
            {
                if (restr[i][ind_col] <= 0)
                    continue;
                if (min > (restr[i][restr[i].Count - 1] / restr[i][ind_col]))
                {
                    min = restr[i][restr[i].Count - 1] / restr[i][ind_col];
                    ind_row = i;
                }
            }

            if (ind_row == -1)
                return 2;

            basis[ind_row] = ind_col;

            for (int i = 0; i < restr[ind_row].Count; i++)
                if (i != ind_col) restr[ind_row][i] /= restr[ind_row][ind_col];
            restr[ind_row][ind_col] = 1;

            for (int i = 0; i < restr.Count; i++)
            {
                if (i == ind_row) continue;
                double coef = -restr[i][ind_col];
                for (int j = 0; j < restr[i].Count; j++)
                    restr[i][j] += restr[ind_row][j] * coef;
            }

            double c = (-func[ind_col]);
            for (int i = 0; i < func.Count; i++)
            {
                double t = func[i] + restr[ind_row][i] * c;
                func[i] = t;
            }

            return 0;
        }
        private string Answer(string to, List<double> func)
        {
            string ans = "";
            double t = func[func.Count - 1];
            if (to == "max")
                ans += $"f {to} = {t}\r\n";
            else
                ans += $"f {to} = {-t}\r\n";

            List<double> vars = new List<double>();
            for (int i = 0; i < func.Count - 1; i++)
                vars.Add(0);

            ans += "(x1, ... , xn) = (";
            for (int i = 0; i < basis.Count; i++)
            {
                if (basis[i] > -1)
                    vars[basis[i]] = restr[i][restr[i].Count - 1];
            }

            for (int i = 0; i < vars.Count; i++)
                ans += $"{vars[i]}, ";

            return ans + ")";
        }
    }
}
