using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

namespace OpenMrsTests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
			driver = new ChromeDriver();
		}

		[TearDown]
		public void TearDown()
		{
			driver.Dispose();
		}

		[Test]
		public void LoginTest()
		{
			driver.Navigate().GoToUrl(LoginUrl);
			var usernameInput = driver.FindElement(By.Id("username"));
			var passwordInput = driver.FindElement(By.Id("password"));
			var locationElement = driver.FindElement(By.Id("Inpatient Ward"));

			usernameInput.SendKeys(AdminLogin);
			passwordInput.SendKeys(AdminPassword);
			locationElement.Click();

			var loginButton = driver.FindElement(By.Id("loginButton"));
			stopwatch.Restart();
			loginButton.Click();

			var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			webDriverWait.Until(driver => driver.Url.Contains(HomeUrl));
			stopwatch.Stop();

			Console.WriteLine($"Result: {stopwatch.ElapsedMilliseconds} ms");

			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThanOrEqualTo(1000));
		}

		[Test]
		public void BookVisitTest()
		{
			driver.Navigate().GoToUrl(LoginUrl);
			SetupCookies();
			driver.Navigate().GoToUrl(AddVisitUrl);

			var visitTypeInput = driver.FindElement(By.Name("visitType"));
			visitTypeInput.Click();

			var options = visitTypeInput.FindElements(By.TagName("option"));
			options[1].Click();

			var startDateInput = driver.FindElement(By.Name("startDatetime"));
			startDateInput.Click();

			var todayButton = driver.FindElement(By.ClassName("ui-datepicker-today"));
			todayButton.Click();

			var visitDiv = driver.FindElement(By.XPath("//input[@value='Zapisz']"));
			stopwatch.Restart();
			visitDiv.Click();
			var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			webDriverWait.Until(driver =>
			{
				var visitSaved = driver.FindElement(By.XPath("//div[text()='Visit Saved']"));

				return visitSaved != null;
			});
			stopwatch.Stop();

			Console.WriteLine($"Result: {stopwatch.ElapsedMilliseconds} ms");

			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThanOrEqualTo(1000));
		}

		[Test]
		public void GenerateListOfDiagnosisTest()
		{
			driver.Navigate().GoToUrl(LoginUrl);
			SetupCookies();
			driver.Navigate().GoToUrl(ListOfDiagnosisUrl);

			var startDateInput = driver.FindElement(By.Id("userEnteredParamstartDate"));
			startDateInput.SendKeys("1/1/1000");

			var endDateInput = driver.FindElement(By.Id("userEnteredParamendDate"));
			endDateInput.SendKeys("31/12/9999");

			var visitDiv = driver.FindElement(By.XPath("//input[@value='Request Report']"));
			stopwatch.Restart();
			visitDiv.Click();
			var webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			webDriverWait.Until(driver =>
				driver.Url.Contains("/openmrs-standalone/module/reporting/reports/reportHistoryOpen.form"));
			stopwatch.Stop();

			Console.WriteLine($"Result: {stopwatch.ElapsedMilliseconds} ms");

			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThanOrEqualTo(3000));
		}

		private void SetupCookies()
		{
			var sessionCookie = new Cookie("JSESSIONID", "05448863A55450CD16D0A6B01568EDCE", Host, "/openmrs-standalone/", DateTime.Now.AddDays(10));
			var referenceAppCookie = new Cookie("_REFERENCE_APPLICATION_LAST_USER_", "92668751", Host, "/openmrs-standalone/", DateTime.Now.AddDays(10));
			var lastSessionLocationCookie = new Cookie("referenceapplication.lastSessionLocation", "6", Host, "/openmrs-standalone/", DateTime.Now.AddDays(10));

			driver.Manage().Cookies.AddCookie(sessionCookie);
			driver.Manage().Cookies.AddCookie(referenceAppCookie);
			driver.Manage().Cookies.AddCookie(lastSessionLocationCookie);
		}

		private Stopwatch stopwatch = new();
		private ChromeDriver driver;
		private const string LoginUrl = Prefix + "openmrs-standalone/login.htm";
		private const string AddVisitUrl = Prefix + "openmrs-standalone/admin/visits/visit.form?patientId=1338";
		private const string ListOfDiagnosisUrl = Prefix + "openmrs-standalone/module/reporting/run/runReport.form?reportId=e451ae04-4881-11e7-a919-92ebcb67fe33";
		private const string HomeUrl = Prefix + "openmrs-standalone/referenceapplication/home.page";
		private const string AdminLogin = "admin";
		private const string AdminPassword = "Admin123";
		private const string Prefix = $"http://{Host}:{Port}/";
		private const string Host = "192.168.56.101";
		private const string Port = "8081";

	}
}