@Smoke @Ui
Feature: Shopping cart
  As a saucedemo customer
  I want to add an item to my cart
  So that I can buy it later

  Scenario: Add an item to the cart
    Given I am logged in as the standard user
    When I add "Sauce Labs Backpack" to the cart
    And I open the cart
    Then the cart should contain "Sauce Labs Backpack"
