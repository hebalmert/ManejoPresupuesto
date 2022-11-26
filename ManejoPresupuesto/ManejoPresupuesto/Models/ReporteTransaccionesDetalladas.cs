using System.Xml.Schema;

namespace ManejoPresupuesto.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }

        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(x => x.BalanceDepositos);

        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);

        public decimal Total => BalanceDepositos - BalanceRetiros; 

        public class TransaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }

            public IEnumerable<Transaccion> Transaccions { get; set; }

            public decimal BalanceDepositos => Transaccions.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso)
                .Sum(x => x.Monto);

            public decimal BalanceRetiros => Transaccions.Where(x => x.TipoOperacionId == TipoOperacion.Gasto)
                .Sum(x => x.Monto);
        }
    }
}
