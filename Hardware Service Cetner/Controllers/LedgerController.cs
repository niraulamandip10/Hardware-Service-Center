using System.Security.Claims;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;
using Dapper;
public class LedgerController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public LedgerController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    [HttpGet]
    [Authorize]
    public IActionResult CreateLedger()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateLedger(LedgerModel ledger)
    {
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      
      using var connection = _dbConnectionProvider.CreateConnection();
      var checkLedgername = await connection.QueryFirstOrDefaultAsync("select ledgername  from ledger where  ledgername = @LedgerName",new
      {
          ledger.LedgerName
      });
      var checkLedgerCode = await connection.QueryFirstOrDefaultAsync("select ledgerCode from ledger where ledgerCode = @LedgerCode",new
      {
          ledger.LedgerCode
      });

      if (checkLedgername != null && checkLedgerCode ==null)
      {
          TempData["Error"]="Ledger Already Exists";
          return RedirectToAction("CreateLedger");
      }

      if (checkLedgerCode != null && checkLedgername==null)
      {
          TempData["Error"]="Ledger Code Already Exists";
          return RedirectToAction("CreateLedger");
      }

      if (checkLedgerCode!=null && checkLedgerCode!=null)
      {
          TempData["Error"]="Ledger Name & Ledger Code Already Exists";
          return RedirectToAction("CreateLedger");
          
      }
      if (ModelState.IsValid)
      {
          var createLedger =
                  "Insert Into ledger (ledgername, ledgercode,coaledgerid,recdate,recbyid,description,status) values " +
                  "(@LedgerName , @LedgerCode,@CoaLedgerId,@RecDate,@RecbyID,@Description,@Status)";
              await connection.ExecuteAsync(createLedger, new
              {
                  ledgername = ledger.LedgerName,
                  ledgercode = ledger.LedgerCode,
                  coaledgerid = ledger.CoaLedgerId,
                  RecDate = DateTime.Now,
                  RecById = userId,
                  ledger.Description,
                  Status = 1
              });
              TempData["success"] = "Ledger created Successfully";
              return RedirectToAction("Report");

      }
      



      return View(ledger);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Report()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var ledgerReport = await connection.QueryAsync<LedgerModel>("Select * from ledger");
        return View(ledgerReport);
    }
}