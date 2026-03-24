namespace RealTimePedestrianControlSystem.Models;

public enum PedestrianStatus { Waiting, Crossing, Cleared }

public class Pedestrian
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Direction { get; set; } = "";
    public int WaitTime { get; set; }
    public PedestrianStatus Status { get; set; }
    public long EnteredAt { get; set; }
    public string Age { get; set; } = "";
    public string Speed { get; set; } = "";
}
