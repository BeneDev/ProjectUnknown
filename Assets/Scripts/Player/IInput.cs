/// <summary>
/// The interface, implementing the necessary properties for the controls
/// </summary>

public interface IInput
{
    float Horizontal { get; }

    int Jump { get; }

    bool Dodge { get; }

    bool Shoot { get; }

    bool Interact { get; }
}