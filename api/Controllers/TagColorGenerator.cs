public static class TagColorGenerator
{
    // Color palette
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

    // Hash function
    public static string Generate(string input)
    {
        int hash = input.ToLowerInvariant().Aggregate(0, (acc, c) => acc + c);
        return TagColors[hash % TagColors.Length];
    }
}