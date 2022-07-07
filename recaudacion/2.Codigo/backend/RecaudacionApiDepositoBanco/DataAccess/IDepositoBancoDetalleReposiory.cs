using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;

namespace RecaudacionApiDepositoBanco.DataAccess
{
    public interface IDepositoBancoDetalleReposiory
    {
        Task<IEnumerable<DepositoBancoDetalle>> FindAll(int depositoBancoId);
        Task<DepositoBancoDetalle> FindById(int id);
        Task<DepositoBancoDetalle> FindByNumeroAndFechaAndCuentaCorriente(string numeroDeposito, DateTime fechaDeposito, int cuentaCorrienteId, int clienteId);
        Task<DepositoBancoDetalle> Add(DepositoBancoDetalle DepositoBancoDetalle);
        Task Update(DepositoBancoDetalle DepositoBancoDetalle);
    }
}
