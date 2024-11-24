using System;
using System.Linq;
using System.Collections.Generic;
using Spectre.Console;
using SteamConfigManager.utils;

namespace SteamConfigManager
{
    class Program
    {
        private static readonly string version = "1.0.0";
        private static readonly string githubUrl = "https://github.com/M3th4d0n/SteamConfigManager";

        static void Main()
        {
            while (true)
            {
                ShowApplicationInfo();

                string steamPath = SteamPath.GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    AnsiConsole.MarkupLine("[red]Steam not found.[/]");
                    return;
                }

                var accounts = SteamUserUtils.GetSteamAccounts(steamPath);
                if (accounts == null || accounts.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No Steam accounts found.[/]");
                    return;
                }

                var options = new List<(int Index, string SteamID, string SteamName)>();
                for (int i = 0; i < accounts.Count; i++)
                {
                    string steamID = accounts[i]?.SteamID ?? "Unknown";
                    string steamName = CleanString(accounts[i]?.Name ?? "Unknown");
                    options.Add((i, steamID, steamName));
                }

                var selectedOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select the source account:[/]")
                        .PageSize(10)
                        .AddChoices(options.Select(o => Markup.Escape($"[{o.Index}] SteamID: {o.SteamID}, SteamName: {o.SteamName}"))));

                int selectedIndex = options.First(o => Markup.Escape($"[{o.Index}] SteamID: {o.SteamID}, SteamName: {o.SteamName}") == selectedOption).Index;
                var sourceAccount = accounts[selectedIndex];

                AnsiConsole.MarkupLine($"[green]Source account selected:[/] [yellow]{sourceAccount.Name}[/]");

                var targetOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select the target account:[/]")
                        .PageSize(10)
                        .AddChoices(options.Select(o => Markup.Escape($"[{o.Index}] SteamID: {o.SteamID}, SteamName: {o.SteamName}"))));
                int targetIndex = options.First(o => Markup.Escape($"[{o.Index}] SteamID: {o.SteamID}, SteamName: {o.SteamName}") == targetOption).Index;
                var targetAccount = accounts[targetIndex];

                AnsiConsole.MarkupLine($"[green]Target account selected:[/] [yellow]{targetAccount.Name}[/]");

                AnsiConsole.MarkupLine($"[green]Scanning games for account [yellow]{sourceAccount.Name}[/]:[/]");
                var games = SteamUserUtils.GetGamesForAccount(steamPath, sourceAccount.SteamID);
                if (games == null || games.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No games found.[/]");
                    return;
                }

                var gameOptions = new List<string>();
                foreach (var game in games)
                {
                    gameOptions.Add($"AppID: {game.AppID}");
                }

                var selectedGame = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select a game to transfer:[/]")
                        .PageSize(10)
                        .AddChoices(gameOptions.Concat(new[] { "All games" })));

                if (selectedGame == "All games")
                {
                    SteamConfigTransfer.TransferAllGames(steamPath, sourceAccount.SteamID, targetAccount.SteamID);
                    AnsiConsole.MarkupLine("[green]All data successfully transferred.[/]");
                }
                else
                {
                    var appID = selectedGame.Replace("AppID: ", "").Trim();
                    SteamConfigTransfer.TransferGame(steamPath, sourceAccount.SteamID, targetAccount.SteamID, appID);
                    AnsiConsole.MarkupLine($"[green]Game data [yellow]AppID: {appID}[/] successfully transferred.[/]");
                }

                // Спрашиваем пользователя, хочет ли он перезапустить
                var restartOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Do you want to restart the program or exit?[/]")
                        .AddChoices("Restart", "Exit"));

                if (restartOption == "Exit")
                {
                    AnsiConsole.MarkupLine("[green]Exiting the program. Goodbye![/]");
                    break;
                }
            }
        }

        private static string CleanString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "Unknown";

            return Markup.Escape(new string(input.Where(c => !char.IsControl(c)).ToArray()));
        }

        private static void ShowApplicationInfo()
        {
            AnsiConsole.Write(
                new Panel(
                        $"[yellow]Author:[/] [cyan][link={githubUrl}]m3th4d0n[/][/]\n" +
                        $"[yellow]Current Version:[/] [green]{version}[/]\n" +
                        $"[yellow]GitHub URL:[/] [link={githubUrl}]{githubUrl}[/]")
                    .BorderColor(new Spectre.Console.Color(0, 255, 255))
                    .Header("Application Information")
            );
        }
    }
}
