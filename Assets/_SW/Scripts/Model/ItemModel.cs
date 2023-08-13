public class ItemModel
{
    public static bool SameType(ItemModel one, ItemModel two)
    {
        return one.GetType() == two.GetType();
    }

    private static bool IsEmpty(ItemModel model) => model is EmptyModel;

    public static bool IsNotEmpty(ItemModel model) => !IsEmpty(model);
}