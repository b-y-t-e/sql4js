using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class Stopers
{
    private Dictionary<String, Stoper> list = new Dictionary<String, Stoper>();

    public Stoper this[String name]
    {
        get { return this.Get(name); }
    }

    public Stoper Get(String Name)
    {
        if (!list.ContainsKey(Name))
            list[Name] = new Stoper();
        return list[Name];
    }

    public override string ToString()
    {
        StringBuilder str = new StringBuilder();
        foreach (var name in list.Keys)
        {
            str.Append(name).Append(" = ").Append(list[name].Average.TotalMilliseconds).Append("[ms];").Append(Environment.NewLine);
        }
        return str.ToString();
    }

    public virtual string ToString(Int32 Count)
    {
        StringBuilder str = new StringBuilder();
        foreach (var name in list.Keys)
        {
            str.Append(name).Append(" = ").Append(list[name].Result.TotalMilliseconds / Count).Append("[ms];").Append(Environment.NewLine);
        }
        return str.ToString();
    }
}

public class Stoper
{
    class StoperItem
    {
        public Int64 Start;

        public Int64 End;

        public TimeSpan? Result
        {
            get
            {
                if (Start > 0 && End > 0)
                    return new TimeSpan(End - Start);
                return null;
            }
        }
    }

    public Int32 Count
    {
        get { return pomiary.Count; }
    }

    public TimeSpan Average
    {
        get { return pomiary.Count > 0 ? TimeSpan.FromMilliseconds(Result.TotalMilliseconds / (double)pomiary.Count) : new TimeSpan(); }
    }

    ////////////////

    private List<StoperItem> pomiary = new List<StoperItem>();

    ////////////////

    private StoperItem GetActual()
    {
        if (pomiary.Count > 0)
        {
            var lLast = pomiary[pomiary.Count - 1];
            if (lLast != null && lLast.Result == null)
            {
                return lLast;
            }
        }
        return null;
    }

    ////////////////

    public void Measure(Action Action)
    {
        try
        {
            Start();
            Action();
        }
        catch
        {
            throw;
        }
        finally
        {
            Pause();
        }
    }

    public T Measure<T>(Func<T> Func)
    {
        T lResult = default(T);
        try
        {
            Start();
            lResult = Func();
        }
        catch
        {
            throw;
        }
        finally
        {
            Pause();
        }
        return lResult;
    }

    ////////////////

    public void Start()
    {
        var lAktualnyPomiar = GetActual();
        if (lAktualnyPomiar == null)
        {
            pomiary.Add(new StoperItem()
            {
                Start = DateTime.Now.Ticks
            });
        }
    }

    public TimeSpan Pause()
    {
        var lAktualnyPomiar = GetActual();
        if (lAktualnyPomiar != null)
        {
            lAktualnyPomiar.End = DateTime.Now.Ticks;
        }
        return this.Result;
    }

    public void Reset()
    {
        pomiary.Clear();
    }

    public TimeSpan Result
    {
        get
        {
            return new TimeSpan(pomiary.Sum(i =>
            {
                var lResult = i.Result;
                if (lResult == null)
                    return 0;
                else
                    return lResult.Value.Ticks;
            }));
        }
    }
}
