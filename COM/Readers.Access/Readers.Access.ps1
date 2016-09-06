cls
Write-Host "Set access/kill password, set locks example"
Write-Host ""
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

function Get-HexString
{
    $([String]::Join(" ", ($args[0] | % { "{0:X2}" -f $_})))
}

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")
if ($connectResult -ne "True")
{
    throw "Can't connect to RFID Bus."
}
Write-Host "Connection esteblished."

$command = Read-Host 'Enter [Y] for setting passwords, locking and for killing'
if ($command -ne 'y' -And $command -ne 'Y')
{
    Write-Host "Terminated by user"
    exit
}

$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
   if ($reader.Mode -eq "StandBy" -And $reader.IsOpen -eq $true)
   {            
       $transponders = $rfidbus.GetTransponders($reader.Id);
       Write-Host "    Reader $($reader.Name) found $($transponders.Count) transponder(s). Reader ID: $($reader.Id)"
          foreach ($transponder in $transponders)
       {    
            Write-Host "    Transponder ID: $(Get-HexString $transponder.Id)"

            $oldAccessPassword = [Byte[]](0,0,0,0)       
            $newAccessPassword = [Byte[]](100, 100, 100, 100)
            $newKillPassword = [Byte[]](100, 100, 100, 100)

            $rfidbus.SetKillPassword($reader.Id, $transponder, $newKillPassword, $oldAccessPassword)
            Write-Host "       Kill password was set. OK."
                    
            $rfidbus.SetAccessPassword($reader.Id, $transponder, $newAccessPassword, $oldAccessPassword)
            Write-Host "       Access password was set. OK."

            
            $epcBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Epc
            $lockTypeLocked = [RfidBus.Com.Primitives.Transponders.TransponderBankLockType]::Locked
            $rfidbus.LockTransponderBank($reader.Id, $transponder, $epcBank, $lockTypeLocked, $newAccessPassword)
            
            Write-Host "       Kill password: $(Get-HexString $newKillPassword)"
            $rfidbus.KillTransponder($reader.Id, $transponder, $newKillPassword)
            Write-Host "       Transponder was killed. OK."
       }
   }
}