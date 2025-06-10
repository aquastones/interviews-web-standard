/// <summary>
/// Utility class for generating consistent tag colors from tag names.
/// Maps tag names to a palette of 10 predefined colors using a hash.
/// </summary>
public static class TagColorGenerator
{
    /// <summary>
    /// Predefined palette of 10 color hex codes for tags.
    /// </summary>
    private static readonly string[] TagColors = new[]
    {
        "#3E5641", // Hunter Green
        "#DD6E42", // Burnt Sienna Orange
        "#59A96A", // Jade Green
        "#FFC43D", // Amber Yellow
        "#5F4BB6", // Iris Blue
        "#5DD9C1", // Turquoise
        "#F97068", // Bittersweet Red
        "#E9D758", // Arylide Yellow
        "#665687", // Ultra Violet
        "#8E518D", // Plum Purple
    };

    /// <summary>
    /// Generates a color hex code from the palette based on the input string (in our case the tag name).
    /// Deterministic, allows collisions
    /// </summary>
    /// <param name="input">Input string (Tag name)</param>
    /// <returns>Hex color code from the palette</returns>
    public static string Generate(string input)
    {
        int hash = input.ToLowerInvariant().Aggregate(0, (acc, c) => acc + c);
        return TagColors[hash % TagColors.Length];
    }
}