using EloBuddy;
using EloBuddy.SDK;

namespace LevelZero.Util
{
    static class ItemUtil
    {
        public static Item GetItem(ItemId id)
        {
            return new Item(id);
        }

        public static Item GetItem(int id)
        {
            return new Item(id);
        }

        public static Item GetItem(ItemId id, int range)
        {
            return new Item(id, range);
        }

        public static Item GetItem(int id, int range)
        {
            return new Item(id, range);
        }
    }
}
