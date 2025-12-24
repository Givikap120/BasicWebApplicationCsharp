$connectionString = "Data Source=(local)\SQLEXPRESS;Initial Catalog=BasicWebApplicationDatabase;Persist Security Info=True;User ID=test_db_admin;Password=admin;Pooling=False;Encrypt=True;Trust Server Certificate=True"
$outputDir = "Entities"
$contextName = "AppDbContext"

Scaffold-DbContext `
    $connectionString `
    Microsoft.EntityFrameworkCore.SqlServer `
    -OutputDir $outputDir `
    -Context $contextName `
    -DataAnnotations `
    -Force