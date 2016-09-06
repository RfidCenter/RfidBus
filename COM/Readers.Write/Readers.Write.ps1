cls
Write-Host "Write data example"
Write-Host ""

function Get-HexString 
{
    $([String]::Join(" ", ($args[0] | % { "{0:X2}" -f $_})))
}


$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")
if (!$connectResult)
{
    throw "Can't connect to RFID Bus."   
}

Write-Host "Connection esteblished."

$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
    if ($reader.Mode -eq "StandBy" -And $reader.IsOpen -eq $true)
    {            
        $transponders = $rfidbus.GetTransponders($reader.Id);
        Write-Host "# Reader $($reader.Name) found $($transponders.Count) transponder(s). Reader ID: $(Get-HexString $reader.Id)"

        foreach ($tag in $transponders)
        {
            $bank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::UserMemory
            $data = [Byte[]](255, 255, 255, 255)
            $bankAddress = 0
            $accessPassword = [Byte[]](0,0,0,0)
            
            $rfidbus.WriteMultipleBlocks($reader.Id, $tag, $bank, $data, $bankAddress, $accessPassword)
            Write-Host "Data '$(Get-HexString $data)' were written on tag '$(Get-HexString $tag.Id)'"
            break
        }

        $transponders = $rfidbus.GetTransponders($reader.Id);
        foreach ($tag in $transponders)
        {
            try {

                $rfidbus.WriteEpcSgtin96($reader.Id, $tag, 
					    461000232,                  # GCP
					    1,                          # Item
					    (Get-Random -Maximum 1000), # Serial
					    0,                          # Epc Filter. 0 - All Others; see standard.
					    3);                         # Partition

                Write-Host "SGTIN-96 has been written on tag '$(Get-HexString $tag.Id)'"
                break
            }
            catch {}            
        }

        $transponders = $rfidbus.GetTransponders($reader.Id);
        foreach ($tag in $transponders)
        {
            try {
                $rfidbus.WriteEpcSscc96($reader.Id, $tag,
				        461000232,	                 # GCP
				        (Get-Random -Maximum 1000),	 # Serial
				        0, 			                 # Epc Filter. 0 - All Others; see standard.
				        3);			                 # Partition

                Write-Host "SSCC-96 has been written on tag '$(Get-HexString $tag.Id)'"
                break
            }
            catch {}            
        }

        $transponders = $rfidbus.GetTransponders($reader.Id);
        foreach ($tag in $transponders)
        {
            try {
                $rfidbus.WriteEpcGiai96($reader.Id, $tag, 
					    461000232,	                 # GCP
					    (Get-Random -Maximum 1000),	 # Asset
					    0, 			                 # Epc Filter. 0 - All Others; see standard.
					    3);			                 # Partition
                Write-Host "GIAI-96 has been written on tag '$(Get-HexString $tag.Id)'"
                break
            }
            catch {}            
        }
    }
}