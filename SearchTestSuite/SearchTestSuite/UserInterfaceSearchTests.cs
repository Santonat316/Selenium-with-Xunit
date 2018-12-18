using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace SearchTestSuite
{
    /// <inheritdoc />
    public class UserInterfaceSearchTests : IDisposable
    {
        private readonly ChromeDriver _driver;

        /// <summary>
        ///     Initialize the chrome driver with explicit arguments
        /// </summary>
        public UserInterfaceSearchTests()
        {
            var chromeOptions = new ChromeOptions();
            var proxy = new Proxy {HttpProxy = "localhost"};
            chromeOptions.Proxy = proxy;
            chromeOptions.AddArgument("--no-referrers");
            chromeOptions.AddArgument("--x-funda-exercise");
            chromeOptions.AddArgument("--reduce-security-for-testing");
            chromeOptions.AddArgument("--app-id=idgpnmonknjnojddfkpgkljpfnnfcklj");
            chromeOptions.AcceptInsecureCertificates = true;

            _driver = new ChromeDriver(chromeOptions);
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }

        [Theory]
        [InlineData(true, "option[3]", "option[5]")] //positive flow
        [InlineData(false, "option[5]", "option[3]")]
        //negative flow
        public void TestBuy(bool searchResultsExpected, string priceFromValue, string priceToValue)
        {
            //Arrange
            _driver.Navigate().GoToUrl("https://www.funda.nl");

            var searchItemBuy =
                _driver.FindElement(By.XPath("//*[@class='search-block__navigation-item' and @href='/koop/']"));
            var locationText = _driver.FindElement(By.XPath("//*[@name='filter_location']"));
            var rangeFromLocation = _driver.FindElement(By.XPath("//*[@name='filter_Straal']"));
            var minimumPriceElement =
                _driver.FindElement(
                    By.XPath("//*[@class='range-filter-selector__select' and @name='filter_KoopprijsVan']"));

            var maximumPriceElement =
                _driver.FindElement(
                    By.XPath("//*[@class='range-filter-selector__select' and @name='filter_KoopprijsTot']"));
            var searchButton = _driver.FindElement(By.XPath("//*[@class='button-primary-alternative']"));


            Assert.True(searchItemBuy.Displayed);
            Assert.True(locationText.Displayed);
            Assert.True(rangeFromLocation.Displayed);
            Assert.True(minimumPriceElement.Displayed);
            Assert.True(maximumPriceElement.Displayed);
            Assert.True(searchButton.Displayed);


            var chosenLocationRange = _driver.FindElement(By.XPath("//*[@name='filter_Straal']/option[2]"));

            var priceFrom = _driver.FindElement(By.XPath(
                "//*[@class='range-filter-selector__select' and @name='filter_KoopprijsVan']" + priceFromValue));
            var priceTo =
                _driver.FindElement(By.XPath(
                    "//*[@class='range-filter-selector__select' and @name='filter_KoopprijsTot']" + priceToValue));
            Assert.NotNull(chosenLocationRange);
            Assert.NotNull(priceFrom);
            Assert.NotNull(priceTo);

            //Act
            searchItemBuy.Click();
            locationText.SendKeys("Den Haag");
            rangeFromLocation.SendKeys(chosenLocationRange.GetAttribute("value"));
            minimumPriceElement.SendKeys(priceFrom.GetAttribute("value"));
            maximumPriceElement.SendKeys(priceTo.GetAttribute("value"));
            searchButton.Click();

            _driver.Navigate().Refresh();

            var resultsComponent = _driver.FindElements(By.XPath("//*[@class='search-output-result-count']/span[1]"));

            //Assert
            Assert.NotNull(resultsComponent);
            var results = false;
            if (searchResultsExpected) results = !resultsComponent[0].Text.Contains("0 resultaten");


            Assert.Equal(searchResultsExpected, results);
        }

        [Theory]
        [InlineData(true, "option[5]", "option[3]")] //positive flow
        [InlineData(false, "option[3]", "option[5]")]
        //negative flow
        public void TestHire(bool searchResultsExpected, string priceFromValue, string priceToValue)
        {
            //Arrange

            _driver.Navigate().GoToUrl("https://www.funda.nl/huur/");
            var searchItemRent =
                _driver.FindElement(By.XPath("//*[@class='search-block__navigation-item' and @href='/huur/']"));
            var locationText = _driver.FindElement(By.XPath("//*[@name='filter_location']"));
            var rangeFromLocation = _driver.FindElement(By.XPath("//*[@name='filter_Straal']"));
            var minimumPriceElement =
                _driver.FindElement(
                    By.XPath("//*[@class='range-filter-selector__select' and @name='filter_HuurprijsVan']"));

            var maximumPriceElement =
                _driver.FindElement(
                    By.XPath("//*[@class='range-filter-selector__select' and @name='filter_HuurprijsTot']"));
            var searchButton = _driver.FindElement(By.XPath("//*[@class='button-primary-alternative']"));

            Assert.True(searchItemRent.Displayed);
            Assert.True(searchItemRent.Displayed);
            Assert.True(locationText.Displayed);
            Assert.True(rangeFromLocation.Displayed);
            Assert.True(minimumPriceElement.Displayed);
            Assert.True(maximumPriceElement.Displayed);
            Assert.True(searchButton.Displayed);

            var chosenLocationRange = _driver.FindElement(By.XPath("//*[@name='filter_Straal']/option[2]"));

            var priceFrom = _driver.FindElement(By.XPath(
                "//*[@class='range-filter-selector__select' and @name='filter_HuurprijsVan']" + priceFromValue));
            var priceTo =
                _driver.FindElement(By.XPath(
                    "//*[@class='range-filter-selector__select' and @name='filter_HuurprijsTot']" + priceToValue));
            Assert.NotNull(chosenLocationRange);
            Assert.NotNull(priceFrom);
            Assert.NotNull(priceTo);

            //Act
            locationText.SendKeys("Den Haag");
            rangeFromLocation.SendKeys(chosenLocationRange.GetAttribute("value"));
            minimumPriceElement.SendKeys(priceFrom.GetAttribute("value"));
            maximumPriceElement.SendKeys(priceTo.GetAttribute("value"));
            searchButton.Click();

            _driver.Navigate().Refresh();


            //Assert
            var resultsComponent = _driver.FindElements(By.XPath("//*[@class='search-output-result-count']/span[1]"));
            Assert.NotNull(resultsComponent);

            var results = false;
            if (searchResultsExpected) results = !resultsComponent[0].Text.Contains("0 resultaten");

            Assert.Equal(searchResultsExpected, results);
        }

        [Theory]
        [InlineData(true, "Den Haag", "nieuwbouw")] //positive flow
        [InlineData(false, "Dusseldorf", "nieuwbouw")] //negative flow
        [InlineData(true, "Den Haag", "recreatie")] //positive flow
        [InlineData(false, "Dusseldorf", "recreatie")]
        //negative flow
        public void TestNewConstructionOrRecreation(bool searchResultsExpected, string searchLocation, string type)
        {
            //Arrange

            _driver.Navigate().GoToUrl("https://www.funda.nl/" + type);
            var searchItemNewConstruction =
                _driver.FindElement(By.XPath($"//*[@class='search-block__navigation-item' and @href='//{type}//']"));
            var locationText = _driver.FindElement(By.XPath("//*[@name='filter_location']"));
            var rangeFromLocation = _driver.FindElement(By.XPath("//*[@name='filter_Straal']"));
            var searchButton = _driver.FindElement(By.XPath("//*[@class='button-primary-alternative']"));
            Assert.True(searchItemNewConstruction.Displayed);
            Assert.True(locationText.Displayed);
            Assert.True(rangeFromLocation.Displayed);
            Assert.True(searchButton.Displayed);

            //Act
            var chosenLocationRange = _driver.FindElement(By.XPath("//*[@name='filter_Straal']/option[2]"));
            locationText.SendKeys(searchLocation);
            rangeFromLocation.SendKeys(chosenLocationRange.GetAttribute("value"));
            searchButton.Click();

            //Assert
            if (searchResultsExpected)
            {
                var resultsComponent =
                    _driver.FindElements(By.XPath("//*[@class='search-output-result-count']/span[1]"));
                Assert.NotNull(resultsComponent);

                var results = resultsComponent[0].Text.Contains("resultaten");
                Assert.Equal(searchResultsExpected, results);
            }
            else
            {
                var errorMessageElement =
                    _driver.FindElementByXPath("//*[@class='autocomplete-no-suggestion-message']");
                Assert.NotNull(errorMessageElement);
                Assert.Contains("Ai! Deze locatie kunnen we helaas niet vinden.", errorMessageElement.Text);
            }
        }


        [Theory]
        [InlineData(true, "belgie")] //positive flow
        [InlineData(false, "albanie")]
        //negative flow
        public void TestEurope(bool searchResultsExpected, string countryInput)
        {
            //Arrange

            _driver.Navigate().GoToUrl("https://www.funda.nl");
            var searchItemEurope =
                _driver.FindElement(By.XPath("//*[@class='search-block__navigation-item' and @href='/europe/']"));
            Assert.True(searchItemEurope.Displayed);
            searchItemEurope.Click();

            var selectCountryTextBox = _driver.FindElement(By.XPath("//*[@class='custom-select-box']"));
            Assert.True(selectCountryTextBox.Displayed);
            Assert.True(selectCountryTextBox.Enabled);

            var selectCountryDropDownButton =
                _driver.FindElementByXPath("//*[@class='icon-arrow-down-blue custom-select-icon']");
            Assert.True(selectCountryDropDownButton.Displayed);
            Assert.True(selectCountryDropDownButton.Enabled);
            var searchButton = _driver.FindElement(By.XPath("//*[@class='button-primary-alternative']"));

            //Act
            selectCountryDropDownButton.Click();
            var listOfCountries = _driver.FindElements(By.XPath("//*[@class='selectbox-list is-open']/li"));

            for (var i = 1; i < listOfCountries.Count; i++)
                if (listOfCountries[i].GetAttribute("value").Equals(countryInput))
                {
                    listOfCountries[i].Click();
                    break;
                }

            searchButton.Click();

            //Assert
            var resultsComponent = _driver.FindElements(By.XPath("//*[@class='search-output-result-count']/span[1]"));
            Assert.NotNull(resultsComponent);

            var results = false;
            if (searchResultsExpected) results = !resultsComponent[0].Text.Contains("0 resultaten");

            Assert.Equal(searchResultsExpected, results);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) _driver.Close();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}