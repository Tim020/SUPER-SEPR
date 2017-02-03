// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public class ResourceGroup {

    public int food;
    public int energy;
    public int ore;

    public ResourceGroup(int food = 0, int energy = 0, int ore = 0) {
        this.food = food;
        this.energy = energy;
        this.ore = ore;
    }

    public static ResourceGroup operator +(ResourceGroup c1, ResourceGroup c2) {
        return new ResourceGroup(c1.getFood() + c2.getFood(),
            c1.getEnergy() + c2.getEnergy(),
            c1.getOre() + c2.getOre());
    }

    public static ResourceGroup operator -(ResourceGroup c1, ResourceGroup c2) {
        return new ResourceGroup(c1.getFood() - c2.getFood(),
            c1.getEnergy() - c2.getEnergy(),
            c1.getOre() - c2.getOre());
    }

    public static ResourceGroup operator *(ResourceGroup c1, ResourceGroup c2) {
        return new ResourceGroup(c1.getFood() * c2.getFood(),
            c1.getEnergy() * c2.getEnergy(),
            c1.getOre() * c2.getOre());
    }

    public static ResourceGroup operator *(ResourceGroup r, int s) {
        return new ResourceGroup(r.getFood() * s,
            r.getEnergy() * s,
            r.getOre() * s);
    }

    public override string ToString() {
        return "ResourceGroup(" + food + ", " + energy + ", " + ore + ")";
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        ResourceGroup resourcesToCompare = (ResourceGroup) obj;

        return food == resourcesToCompare.food &&
               energy == resourcesToCompare.energy &&
               ore == resourcesToCompare.ore;
    }

    public override int GetHashCode() {
        return food.GetHashCode() ^ energy.GetHashCode() << 2 ^ ore.GetHashCode() >> 2;
    }

    public int Sum() {
        return food + energy + ore;
    }

    public int getFood() {
        return food;
    }

    public int getEnergy() {
        return energy;
    }

    public int getOre() {
        return ore;
    }

}