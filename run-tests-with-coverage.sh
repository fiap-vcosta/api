#!/bin/bash

# Run tests with coverage
echo "Running tests with code coverage..."
dotnet test --settings coverlet.runsettings --results-directory ./test-results --logger "console;verbosity=detailed"

# Generate HTML report
echo "Generating HTML coverage report..."
reportgenerator -reports:"./test-results/*/coverage.cobertura.xml" -targetdir:"./test-results/coverage-report" -reporttypes:Html

echo "Coverage report generated at: ./test-results/coverage-report/index.html"
echo "Open the HTML file in your browser to view the coverage report."