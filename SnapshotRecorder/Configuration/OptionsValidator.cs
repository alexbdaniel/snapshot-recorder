using MiniValidation;

namespace SnapshotRecorder.Configuration;

public static class OptionsValidator
{
    public static bool Validate<TModel>(TModel model)
    {
        bool valid = MiniValidator.TryValidate(model, out IDictionary<string, string[]> errors);
        
        Console.WriteLine($"{nameof(model)} has one or more validation errors:");
        foreach (var entry in errors)
        {
            Console.WriteLine($"  {entry.Key}:");
            foreach (var error in entry.Value)
            {
                Console.WriteLine($"  - {error}");
            }
        }
        
        if (!valid)
            Environment.Exit(1);
        
        return valid;
    }
}