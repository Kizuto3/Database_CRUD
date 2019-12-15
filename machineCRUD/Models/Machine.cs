namespace machineCRUD.Models
{
    public class Machine
    {
        public int MachineId { get; set; }
        public string Producer { get; set; }
        public string Type { get; set; }
        public float Price { get; set; }
        public int Flops { get; set; }

        public override string ToString()
        {
            return $"{MachineId} \t {Producer} {Type} \t {Price} \t {Flops}";
        }
    }
}