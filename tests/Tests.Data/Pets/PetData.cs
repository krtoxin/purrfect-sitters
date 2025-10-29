using Domain.Pets;
using System;

namespace Tests.Data.Pets;

public static class PetData
{
    public static Pet FirstPet(Guid ownerId)
        => Pet.Create(Guid.NewGuid(), ownerId, "Fluffy", PetType.Cat, "British Shorthair", "Very friendly");

    public static Pet SecondPet(Guid ownerId)
        => Pet.Create(Guid.NewGuid(), ownerId, "Rex", PetType.Dog, "Labrador", "Loves walks");

    public static Pet ThirdPet(Guid ownerId)
        => Pet.Create(Guid.NewGuid(), ownerId, "Tweety", PetType.Bird, "Canary", "Sings beautifully");

    public static Pet FourthPet(Guid ownerId)
        => Pet.Create(Guid.NewGuid(), ownerId, "Luna", PetType.Cat, "Persian", "Needs daily grooming");

    public static Pet FifthPet(Guid ownerId)
        => Pet.Create(Guid.NewGuid(), ownerId, "Max", PetType.Dog, "German Shepherd", "Well trained, protective");

    public static Pet RandomPet(Guid ownerId)
    {
        var names = new[] { "Bella", "Charlie", "Lucy", "Cooper", "Milo", "Oliver", "Leo", "Loki", "Zeus", "Nova" };
        var breeds = new[] 
        { 
            (PetType.Cat, "Siamese"),
            (PetType.Cat, "Maine Coon"),
            (PetType.Dog, "Golden Retriever"),
            (PetType.Dog, "Poodle"),
            (PetType.Bird, "Parakeet"),
            (PetType.Bird, "Cockatiel")
        };

        var random = new Random();
        var name = names[random.Next(names.Length)];
        var (type, breed) = breeds[random.Next(breeds.Length)];
        var notes = type switch
        {
            PetType.Cat => "Independent and affectionate",
            PetType.Dog => "Energetic and friendly",
            PetType.Bird => "Cheerful and social",
            _ => "Lovely pet"
        };

        return Pet.Create(Guid.NewGuid(), ownerId, name, type, breed, notes);
    }

    public static IEnumerable<Pet> CreateMany(Guid ownerId, int count)
    {
        var pets = new List<Pet>
        {
            FirstPet(ownerId),
            SecondPet(ownerId),
            ThirdPet(ownerId),
            FourthPet(ownerId),
            FifthPet(ownerId)
        };

        while (pets.Count < count)
        {
            pets.Add(RandomPet(ownerId));
        }

        return pets.Take(count);
    }
}
