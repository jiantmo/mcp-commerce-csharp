#!/usr/bin/env pwsh

# Simple test script for D365 Commerce MCP Server
# This script tests the basic functionality of the MCP server

Write-Host "Starting D365 Commerce MCP Server test..." -ForegroundColor Green

# Start the server in the background
$serverProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "McpCommerceServer.csproj" -NoNewWindow -PassThru -RedirectStandardInput -RedirectStandardOutput

# Wait a moment for the server to start
Start-Sleep -Seconds 2

Write-Host "Server started with PID: $($serverProcess.Id)" -ForegroundColor Yellow

try {
    # Test 1: Initialize
    Write-Host "`nTest 1: Initialize" -ForegroundColor Cyan
    $initRequest = @{
        jsonrpc = "2.0"
        id = 1
        method = "initialize"
        params = @{
            protocolVersion = "2025-06-18"
            capabilities = @{}
            clientInfo = @{
                name = "test-client"
                version = "1.0.0"
            }
        }
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Sending: $initRequest" -ForegroundColor Gray
    
    # Test 1.5: Send initialized notification
    Write-Host "`nTest 1.5: Send Initialized Notification" -ForegroundColor Cyan
    $initializedNotification = @{
        jsonrpc = "2.0"
        method = "notifications/initialized"
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Sending: $initializedNotification" -ForegroundColor Gray
    
    # Test 2: List Tools
    Write-Host "`nTest 2: List Tools" -ForegroundColor Cyan
    $listToolsRequest = @{
        jsonrpc = "2.0"
        id = 2
        method = "tools/list"
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Sending: $listToolsRequest" -ForegroundColor Gray
    
    # Test 3: Call Customer Search
    Write-Host "`nTest 3: Customer Search" -ForegroundColor Cyan
    $customerSearchRequest = @{
        jsonrpc = "2.0"
        id = 3
        method = "tools/call"
        params = @{
            name = "customer_search"
            arguments = @{
                customerSearchCriteria = @{
                    searchText = "john doe"
                    email = "john.doe@example.com"
                }
                queryResultSettings = @{
                    paging = @{
                        skip = 0
                        top = 10
                    }
                    sorting = @{
                        field = "FullName"
                        isDescending = $false
                    }
                }
            }
        }
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Sending: $customerSearchRequest" -ForegroundColor Gray
    
    Write-Host "`nTests prepared. To manually test:" -ForegroundColor Green
    Write-Host "1. Run: dotnet run --project McpCommerceServer.csproj" -ForegroundColor White
    Write-Host "2. Send the JSON requests shown above to the server's stdin" -ForegroundColor White
    Write-Host "3. Observe the responses on stdout" -ForegroundColor White
    
} finally {
    # Clean up
    if ($serverProcess -and !$serverProcess.HasExited) {
        Write-Host "`nStopping server..." -ForegroundColor Yellow
        $serverProcess.Kill()
        $serverProcess.WaitForExit(5000)
    }
}

Write-Host "`nTest script completed!" -ForegroundColor Green
