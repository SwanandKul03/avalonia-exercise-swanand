using System;

namespace AvaloniaExercise.Models;

public enum PedestrianSpecies
{
    Human,
    Cow,
    Dog,
    Hippo,
    Otter,
    Cat,
    Crow,
    Dove,
    Dragon,
    Fish,
    Frog,
    Horse,
    KiwiBird,
    Locust,
    Mosquito,
    Shrimp,
    Spider,
    Worm
}

public static class PedestrianSpeciesExtensions
{
    public static string ToDisplayName(this PedestrianSpecies species)
    {
        return species switch
        {
            PedestrianSpecies.Human => "Human",
            PedestrianSpecies.Cow => "Cow",
            PedestrianSpecies.Dog => "Dog",
            PedestrianSpecies.Hippo => "Hippo",
            PedestrianSpecies.Otter => "Otter",
            PedestrianSpecies.Cat => "Cat",
            PedestrianSpecies.Crow => "Crow",
            PedestrianSpecies.Dove => "Dove",
            PedestrianSpecies.Dragon => "Dragon",
            PedestrianSpecies.Fish => "Fish",
            PedestrianSpecies.Frog => "Frog",
            PedestrianSpecies.Horse => "Horse",
            PedestrianSpecies.KiwiBird => "Kiwi Bird",
            PedestrianSpecies.Locust => "Locust",
            PedestrianSpecies.Mosquito => "Mosquito",
            PedestrianSpecies.Shrimp => "Shrimp",
            PedestrianSpecies.Spider => "Spider",
            PedestrianSpecies.Worm => "Worm",
            _ => throw new ArgumentOutOfRangeException(nameof(species), species, null)
        };
    }

    public static string ToIconName(this PedestrianSpecies species)
    {
        return species switch
        {
            PedestrianSpecies.Human => "fa-person",
            PedestrianSpecies.Cow => "fa-cow",
            PedestrianSpecies.Dog => "fa-dog",
            PedestrianSpecies.Hippo => "fa-hippo",
            PedestrianSpecies.Otter => "fa-otter",
            PedestrianSpecies.Cat => "fa-cat",
            PedestrianSpecies.Crow => "fa-crow",
            PedestrianSpecies.Dove => "fa-dove",
            PedestrianSpecies.Dragon => "fa-dragon",
            PedestrianSpecies.Fish => "fa-fish",
            PedestrianSpecies.Frog => "fa-frog",
            PedestrianSpecies.Horse => "fa-horse",
            PedestrianSpecies.KiwiBird => "fa-kiwi-bird",
            PedestrianSpecies.Locust => "fa-locust",
            PedestrianSpecies.Mosquito => "fa-mosquito",
            PedestrianSpecies.Shrimp => "fa-shrimp",
            PedestrianSpecies.Spider => "fa-spider",
            PedestrianSpecies.Worm => "fa-worm",
            _ => throw new ArgumentOutOfRangeException(nameof(species), species, null)
        };
    }
}
