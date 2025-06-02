namespace Zuul;

class SpellRegistry {
    private static Dictionary<string, Spell> registry = new Dictionary<string, Spell>();

    public static void register(Spell spell) {
        registry.TryAdd(spell.Name, spell);
    }

    public static Spell get(string name) {
        return registry[name];
    }
}