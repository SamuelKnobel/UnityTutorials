using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Race
{
     [System.NonSerialized]
    public GameControll game;
    public enum Peoples
    {
        Nobody = 0,
        Angmar = 1,
        Gondor = 2,
        Harad = 3,
        Isengart = 4,
        Khazad = 5, // Zwerge
        Mordor = 6,
        Noldor = 7,
        Numenor = 8,
        Rhun = 9,  // Ostlinge
        Rohan = 10
    }
    //[System.NonSerialized]

    public Peoples peoples;

    public int actualMoney;
    public int MoneyLastRound;
    public bool b_TaxCollected;
    public bool b_GoldCollected;
    public int MoneyEarnedLastRound;

    [System.NonSerialized]
    public List<Army> armies;

    public List<Buildings> ownedBuildings = new List<Buildings>();
    
    public void generateRaces()
    {
        game = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();

        game.playableRaces.Add(new Nobody());
        game.playableRaces.Add(new Angmar());
        game.playableRaces.Add(new Gondor());
        game.playableRaces.Add(new Harad());
        game.playableRaces.Add(new Isengart());
        game.playableRaces.Add(new Khazad());
        game.playableRaces.Add(new Mordor());
        game.playableRaces.Add(new Noldor());
        game.playableRaces.Add(new Numenor());
        game.playableRaces.Add(new Rhun());
        game.playableRaces.Add(new Rohan());
    }
}
[System.Serializable]
public class Nobody : Race
{
    public Nobody()
    {
        peoples = Peoples.Nobody;
    }
    //~Nobody()
    //{

    //}
}
[System.Serializable]
public class Angmar : Race
{
    public Angmar()
    {
        peoples = Peoples.Angmar;
    }
}
[System.Serializable]
public class Gondor : Race
{
    public Gondor()
    {
        peoples = Peoples.Gondor;
    }
}
[System.Serializable]
public class Harad : Race
{
    public Harad()
    {
        peoples = Peoples.Harad;
    }
}
public class Isengart : Race
{
    public Isengart()
    {
        peoples = Peoples.Isengart;
    }
}
public class Khazad : Race
{
    public Khazad()
    {
        peoples = Peoples.Khazad;
    }
}
public class Mordor : Race
{
    public Mordor()
    {
        peoples = Peoples.Mordor;
    }
}
public class Noldor : Race
{
    public Noldor()
    {
        peoples = Peoples.Noldor;
    }
}
public class Numenor : Race
{
    public Numenor()
    {
        peoples = Peoples.Numenor;
    }
}
public class Rhun : Race
{
    public Rhun()
    {
        peoples = Peoples.Rhun;
    }
}
public class Rohan : Race
{
    public Rohan()
    {
        peoples = Peoples.Rohan;
    }
}

