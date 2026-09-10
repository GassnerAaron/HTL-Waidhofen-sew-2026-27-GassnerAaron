interface IFigur
{
    double BerechneOberflaeche();
    double BerechneVolumen();
}

abstract class Figur : IFigur
{
    protected Figur(string beschreibung)
    {
        Beschreibung = beschreibung;
    }

    public string Beschreibung { get; }

    public abstract double BerechneOberflaeche();
    public abstract double BerechneVolumen();
}

class Kugel : Figur
{
    public Kugel(string beschreibung, double radius) : base(beschreibung)
    {
        Radius = radius;
    }

    public double Radius { get; }

    public override double BerechneOberflaeche()
    {
        return 4.0 * Math.PI * Math.Pow(Radius, 2);
    }

    public override double BerechneVolumen()
    {
        return 4.0 * Math.PI * Math.Pow(Radius, 3) / 3.0;
    }
}

class Wuerfel : Figur
{
    public Wuerfel(string beschreibung, double seitenlaenge) : base(beschreibung)
    {
        Seitenlaenge = seitenlaenge;
    }

    public double Seitenlaenge { get; }

    public override double BerechneOberflaeche()
    {
        return 6.0 * Math.Pow(Seitenlaenge, 2);
    }

    public override double BerechneVolumen()
    {
        return Math.Pow(Seitenlaenge, 3);
    }
}

class FigurListe
{
    private readonly List<Figur> figuren = new();

    public int Count
    {
        get
        {
            return figuren.Count;
        }
    }

    public void Add(Figur figur)
    {
        ArgumentNullException.ThrowIfNull(figur);
        figuren.Add(figur);
    }

    public void Remove(int index)
    {
        figuren.RemoveAt(index);
    }

    public void Remove(Figur figur)
    {
        figuren.Remove(figur);
    }

    public void AusgabeAllerFiguren()
    {
        foreach (Figur figur in figuren)
        {
            Console.WriteLine(
                $"{figur.Beschreibung}: Oberfläche = {figur.BerechneOberflaeche():F2}, " +
                $"Volumen = {figur.BerechneVolumen():F2}");
        }
    }
}

class Program
{
    static void Main()
    {
        FigurListe liste = new();

        Kugel k1 = new("K1", 10);
        liste.Add(new Kugel("K2", 8));
        liste.Add(new Wuerfel("W1", 3));
        liste.Add(new Wuerfel("W2", 4));
        liste.Add(k1);
        liste.Remove(1);
        liste.Remove(k1);

        Console.WriteLine($"Anzahl der Elemente in der Liste: {liste.Count}");
        liste.AusgabeAllerFiguren();
    }
}
