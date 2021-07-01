public class RiverCard
{
    public ElementData element;
    public bool locked;
    public bool inactive;

    public RiverCard(ElementData element, bool locked, bool inactive)
    {
        this.element = element;
        this.locked = locked;
        this.inactive = inactive;
    }
}