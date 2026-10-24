using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeterYoungWebsiteBackend.API.Data;

namespace PeterYoungWebsiteBackend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestTableController(AppDbContext db, ILogger<TestTableController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Dictionary<string, object?>>>> Get(CancellationToken cancellationToken)
    {
        var connection = db.Database.GetDbConnection();
        var openedHere = connection.State != ConnectionState.Open;

        try
        {
            if (openedHere)
            {
                await db.Database.OpenConnectionAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM testtable";

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var rows = new List<Dictionary<string, object?>>();

            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>(reader.FieldCount, StringComparer.Ordinal);
                for (var column = 0; column < reader.FieldCount; column++)
                {
                    row[reader.GetName(column)] = await reader.IsDBNullAsync(column, cancellationToken)
                        ? null
                        : reader.GetValue(column);
                }

                rows.Add(row);
            }

            return Ok(rows);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Could not read rows from testtable.");
            return Problem("The testtable data could not be loaded.", statusCode: StatusCodes.Status500InternalServerError);
        }
        finally
        {
            if (openedHere)
            {
                await db.Database.CloseConnectionAsync();
            }
        }
    }
}
