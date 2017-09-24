REM - First delete any existing AssmeblyDeploy folder to clear any files that may have been copied by running V7Deploy_Debug.bat.
rmdir .\_deploy\ /S /Q

REM - Copy RecipeBox.Model and dependencies to Admin and Web folders
xcopy .\RecipeBox.Model\bin\Debug\Newtonsoft.Json.dll .\_deploy\Admin\ /Y
xcopy .\RecipeBox.Model\bin\Debug\RecipeBox.Model.dll .\_deploy\Admin\ /Y
xcopy .\RecipeBox.Model\bin\Debug\Newtonsoft.Json.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.Model\bin\Debug\RecipeBox.Model.dll .\_deploy\Web\bin\ /Y

REM - Copy RecipeBox.Core and dependencies to Admin and Web folders
xcopy .\RecipeBox.Core\bin\Debug\EntityFramework.dll .\_deploy\Admin\ /Y
xcopy .\RecipeBox.Core\bin\Debug\EntityFramework.SqlServer.dll .\_deploy\Admin\ /Y
xcopy .\RecipeBox.Core\bin\Debug\RecipeBox.Core.dll .\_deploy\Admin\ /Y
xcopy .\RecipeBox.Core\bin\Debug\EntityFramework.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.Core\bin\Debug\EntityFramework.SqlServer.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.Core\bin\Debug\RecipeBox.Core.dll .\_deploy\Web\bin\ /Y

REM - Copy RecipeBox.DataContext and dependencies to Admin and Web folders
xcopy .\RecipeBox.DataContext\bin\Debug\RecipeBox.DataContext.dll .\_deploy\Admin\ /Y
xcopy .\RecipeBox.DataContext\bin\Debug\RecipeBox.DataContext.dll .\_deploy\Web\bin\ /Y

REM - Copy RecipeBox.WebApi and dependencies to Web folder
xcopy .\RecipeBox.WebApi\App_Start\* .\_deploy\Web\App_Start\ /Y
xcopy .\RecipeBox.WebApi\Global.asax .\_deploy\Web\ /Y
xcopy .\RecipeBox.WebApi\bin\RecipeBox.WebApi.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\log4net.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\Microsoft.Web.Infrastructure.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\Ninject.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\Ninject.Web.Common.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\System.Net.Http.Formatting.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\System.Web.Http.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\System.Web.Http.WebHost.dll .\_deploy\Web\bin\ /Y
xcopy .\RecipeBox.WebApi\bin\WebActivator.dll .\_deploy\Web\bin\ /Y

REM - Copy RecipeBox.ConsoleApp and dependencies to Web folder
xcopy .\RecipeBox.ConsoleApp\bin\Debug\RecipeBox.ConsoleApp.exe .\_deploy\Admin\ /Y

REM - Copy RecipeBox.Html and dependencies to Web folder
xcopy .\RecipeBox.Html\app\views\* .\_deploy\Web\app\views\ /Y
xcopy .\RecipeBox.Html\Content\* .\_deploy\Web\Content\ /Y
xcopy .\RecipeBox.Html\fonts\* .\_deploy\Web\fonts\ /Y
xcopy .\RecipeBox.Html\Images\* .\_deploy\Web\Images\ /Y
xcopy .\RecipeBox.Html\Scripts\* .\_deploy\Web\Scripts\ /Y
xcopy .\RecipeBox.Html\index.html .\_deploy\Web\ /Y
mkdir .\_deploy\Web\RecipeImages\
