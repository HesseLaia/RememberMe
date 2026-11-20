param(
    [Parameter(Mandatory)][string]$Source,
    [Parameter(Mandatory)][string]$OpenParcel,
    [switch]$OnlyRevision,
    [switch]$CharacterPass,
    [switch]$PolishPass,
    [string]$FourthVisitor = '5742515f-8610-451e-a6dd-2c831ae519d5',
    [string]$ModernMourners,
    [string]$RemainingProps = 'fb718ff6-ebe6-44d2-ac05-c17070089bbe',
    [hashtable]$Story = @{
        Gravestone = '6e1dd792-4c0b-4d03-a1fa-f30f9f4e5728'
        Mechanism = 'eca4e2ab-ebee-439c-acb3-d91510f2b5a4'
        Reunion = 'b1164fa3-9ac4-42a4-b58d-21e28c9f9914'
    }
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$destination = Join-Path $PSScriptRoot 'Assets/Art'
$old = Join-Path $PSScriptRoot '../RememberMeAct1'

Add-Type -ReferencedAssemblies @([Drawing.Bitmap].Assembly.Location, [Drawing.Rectangle].Assembly.Location) -TypeDefinition @'
using System;
using System.Drawing;

public static class SpriteCrop
{
    public static void SharpenImportedSprite(Bitmap bitmap)
    {
        var source = new Color[bitmap.Width, bitmap.Height];
        for (int y = 0; y < bitmap.Height; y++)
        for (int x = 0; x < bitmap.Width; x++) source[x, y] = bitmap.GetPixel(x, y);
        for (int y = 2; y < bitmap.Height - 2; y++)
        for (int x = 2; x < bitmap.Width - 2; x++)
        {
            var c = source[x, y];
            if (c.A < 220) continue;
            var a = source[x - 2, y];
            var b = source[x + 2, y];
            var d = source[x, y - 2];
            var e = source[x, y + 2];
            if (Math.Min(Math.Min(a.A, b.A), Math.Min(d.A, e.A)) < 175) continue;
            bitmap.SetPixel(x, y, Color.FromArgb(c.A,
                Edge(c.R, a.R + b.R + d.R + e.R), Edge(c.G, a.G + b.G + d.G + e.G), Edge(c.B, a.B + b.B + d.B + e.B)));
        }
    }

    static int Edge(int center, int neighbors) => Math.Max(0, Math.Min(255,
        center + Math.Max(-13, Math.Min(13, (int)Math.Round((center - neighbors / 4f) * .82f)))));

    public static void MatchCoat(Bitmap bitmap, int start, int lift)
    {
        for (int y = start; y < bitmap.Height; y++)
        for (int x = 0; x < bitmap.Width; x++)
        {
            var c = bitmap.GetPixel(x, y);
            if (c.A < 87 || c.R >= 91) continue;
            float mask = Math.Min(1, (y - start) / 18f) * Math.Min(1, (91 - c.R) / 24f);
            int red = (int)Math.Round(lift * mask);
            int green = (int)Math.Round((lift - 2) * mask);
            int blue = (int)Math.Round((lift - 1) * mask);
            bitmap.SetPixel(x, y, Color.FromArgb(c.A, c.R + red, c.G + green, c.B + blue));
        }
    }

    public static Rectangle LargestBody(Bitmap bitmap, Rectangle cell)
    {
        int width = cell.Width, height = cell.Height;
        var mask = new byte[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (bitmap.GetPixel(cell.X + x, cell.Y + y).A >= 165) mask[y * width + x] = 1;
        var queue = new int[mask.Length];
        Rectangle body = Rectangle.Empty;
        int largest = 0;
        for (int i = 0; i < mask.Length; i++)
        {
            if (mask[i] != 1) continue;
            int end = 1, left = width, top = height, right = 0, bottom = 0;
            queue[0] = i;
            mask[i] = 0;
            for (int start = 0; start < end; start++)
            {
                int index = queue[start], x = index % width, y = index / width;
                left = Math.Min(left, x); right = Math.Max(right, x);
                top = Math.Min(top, y); bottom = Math.Max(bottom, y);
                if (x > 0 && mask[index - 1] == 1) { mask[index - 1] = 0; queue[end++] = index - 1; }
                if (x + 1 < width && mask[index + 1] == 1) { mask[index + 1] = 0; queue[end++] = index + 1; }
                if (y > 0 && mask[index - width] == 1) { mask[index - width] = 0; queue[end++] = index - width; }
                if (y + 1 < height && mask[index + width] == 1) { mask[index + width] = 0; queue[end++] = index + width; }
            }
            if (end <= largest) continue;
            largest = end;
            body = Rectangle.FromLTRB(cell.X + left, cell.Y + top, cell.X + right + 1, cell.Y + bottom + 1);
        }
        body.Inflate(2, 2);
        return Rectangle.Intersect(cell, body);
    }
}
'@

function Open-Art([string]$id) {
    [Drawing.Bitmap]::FromFile((Join-Path $Source "exec-$id.png"))
}

function Save-Art($sourceArt, [Drawing.Rectangle]$region, [string]$name,
    [int]$width, [int]$height, [int]$imageWidth = 0, [int]$imageHeight = 0, [bool]$flip = $false,
    [bool]$clean = $false, [bool]$sharpen = $false) {
    $file = Join-Path $destination "$name.png"
    New-Item -Force -ItemType Directory -Path (Split-Path -Parent $file) | Out-Null
    if (!$imageWidth) { $imageWidth = $width }
    if (!$imageHeight) { $imageHeight = $height }
    $output = [Drawing.Bitmap]::new($width, $height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $draw = [Drawing.Graphics]::FromImage($output)
    try {
        $draw.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $draw.DrawImage($sourceArt, [Drawing.Rectangle]::new(
            [int][Math]::Floor(($width - $imageWidth) / 2), $height - $imageHeight, $imageWidth, $imageHeight
        ), $region, [Drawing.GraphicsUnit]::Pixel)
        if ($flip) { $output.RotateFlip([Drawing.RotateFlipType]::RotateNoneFlipX) }
        if ($clean) {
            for ($row = 0; $row -lt $height; $row++) {
                for ($column = 0; $column -lt $width; $column++) {
                    $pixel = $output.GetPixel($column, $row)
                    if ($pixel.A -lt 87) { $output.SetPixel($column, $row, [Drawing.Color]::Transparent) }
                }
            }
        }
        if ($sharpen) { [SpriteCrop]::SharpenImportedSprite($output) }
        $output.Save($file, [Drawing.Imaging.ImageFormat]::Png)
    } finally { $draw.Dispose(); $output.Dispose() }
}

function Find-Body($image, [Drawing.Rectangle]$region) {
    $left = $region.Right; $top = $region.Bottom
    $right = $region.Left; $bottom = $region.Top
    for ($y = $region.Top; $y -lt $region.Bottom; $y += 3) {
        for ($x = $region.Left; $x -lt $region.Right; $x += 3) {
            if ($image.GetPixel($x, $y).A -lt 205) { continue }
            $left = [Math]::Min($left, $x); $right = [Math]::Max($right, $x)
            $top = [Math]::Min($top, $y); $bottom = [Math]::Max($bottom, $y)
        }
    }
    if ($right -le $left -or $bottom -le $top) { throw "Empty art at $region" }
    $left = [Math]::Max($region.Left, $left - 3); $top = [Math]::Max($region.Top, $top - 3)
    $right = [Math]::Min($region.Right, $right + 4); $bottom = [Math]::Min($region.Bottom, $bottom + 4)
    [Drawing.Rectangle]::new($left, $top, $right - $left, $bottom - $top)
}

function Cell($image, [int]$col, [int]$row, [int]$columns, [int]$rows) {
    $width = [int][Math]::Floor($image.Width / $columns)
    $height = [int][Math]::Floor($image.Height / $rows)
    [Drawing.Rectangle]::new($col * $width, $row * $height, $width, $height)
}

function Place-Cell($image, $region, [string]$name, [int]$limit) {
    $body = [SpriteCrop]::LargestBody($image, $region)
    if ($body.Width -lt 5 -or $body.Height -lt 5) { throw "Empty prop $name" }
    $factor = $limit / [double][Math]::Max($body.Width, $body.Height)
    Save-Art $image $body $name ([int]($body.Width * $factor)) ([int]($body.Height * $factor)) -clean $true
}

if ($PolishPass) {
    $art = Open-Art 'c5768442-5ab7-4bc9-92b8-df2cdd2adcfb'
    $columns = [ordered]@{
        Idle = @(0)
        Walk = @(1, 0, 2)
        Run = @(3, 4, 5)
        Crouch = @(7, 6, 7)
    }
    try {
        for ($dir = 0; $dir -lt 4; $dir++) {
            $anchor = [SpriteCrop]::LargestBody($art, (Cell $art 0 $dir 8 4))
            $scale = 168.0 / $anchor.Height
            foreach ($set in $columns.GetEnumerator()) {
                for ($step = 0; $step -lt $set.Value.Count; $step++) {
                    $alternateCrouch = $set.Key -eq 'Crouch' -and $step -eq 2
                    $row = if ($alternateCrouch -and $dir -in 1,2) { 3 - $dir } else { $dir }
                    $body = [SpriteCrop]::LargestBody($art, (Cell $art $set.Value[$step] $row 8 4))
                    if ($body.Height -lt 80) { throw "Missing Grace $($set.Key) pose $dir $step" }
                    $factor = [Math]::Min($scale, 172.0 / $body.Height)
                    $index = $dir * $set.Value.Count + $step
                    Save-Art $art $body "Actors/Grace$($set.Key)Polished/$index" 164 172 ([int]($body.Width * $factor)) ([int]($body.Height * $factor)) $alternateCrouch $true
                }
            }
        }
    } finally { $art.Dispose() }

    $groups = @(@{ Id = 'aacfb5e7-c3e0-4e79-b31c-c6c0a39c0faf'; People = @('VisitorC', 'VisitorD') })
    if ($FourthVisitor) { $groups += @{ Id = $FourthVisitor; People = @('VisitorE') } }
    foreach ($group in $groups) {
        $art = Open-Art $group.Id
        try {
            for ($dir = 0; $dir -lt 4; $dir++) {
                for ($visitor = 0; $visitor -lt $group.People.Count; $visitor++) {
                    for ($step = 0; $step -lt 3; $step++) {
                        $body = [SpriteCrop]::LargestBody($art, (Cell $art ($visitor * 3 + $step) $dir ($group.People.Count * 3) 4))
                        if ($body.Height -lt 80) { throw "Missing visitor pose $dir $visitor $step" }
                        $width = [int](208.0 * $body.Width / $body.Height)
                        Save-Art $art $body "Actors/$($group.People[$visitor])Walk/$($dir * 3 + $step)" 164 208 $width 208 -clean $true
                        if ($step -eq 1) { Save-Art $art $body "Actors/$($group.People[$visitor])/$dir" 164 208 $width 208 -clean $true }
                    }
                }
            }
        } finally { $art.Dispose() }
    }

    $art = Open-Art '94efe400-6f09-4123-8c0b-3f5508fd2c49'
    try {
        for ($row = 0; $row -lt 2; $row++) {
            for ($i = 0; $i -lt 4; $i++) {
                $body = [SpriteCrop]::LargestBody($art, (Cell $art $i $row 4 2))
                if ($body.Height -lt 100) { throw "Missing stationary mourner $row $i" }
                $folder = if ($row -eq 0) { 'MournersPolished' } else { 'MournerReserves' }
                Save-Art $art $body "Actors/$folder/$i" 164 208 ([int](208.0 * $body.Width / $body.Height)) 208 -clean $true
            }
        }
    } finally { $art.Dispose() }
    return
}

if ($CharacterPass) {
    $art = Open-Art 'c0ff50c9-a5c5-4106-aad6-8973718afc2d'
    $names = @('UncleHD', 'AuntHD', 'DadCrying')
    try {
        for ($dir = 0; $dir -lt 4; $dir++) {
            for ($column = 0; $column -lt $names.Length; $column++) {
                $body = [SpriteCrop]::LargestBody($art, (Cell $art $column $dir 3 4))
                if ($body.Height -lt 64) { throw "Missing $($names[$column]) pose $dir" }
                Save-Art $art $body "Actors/$($names[$column])/$dir" 164 208 ([int](208.0 * $body.Width / $body.Height)) 208 -clean $true
            }
        }
    } finally { $art.Dispose() }

    $art = Open-Art '9c98d221-85b6-45a1-ab81-0b07eee14d5c'
    $poses = @('GraceWalkUnified', 'GraceRunUnified')
    try {
        for ($dir = 0; $dir -lt 4; $dir++) {
            for ($mode = 0; $mode -lt 2; $mode++) {
                for ($step = 0; $step -lt 3; $step++) {
                    $body = [SpriteCrop]::LargestBody($art, (Cell $art ($mode * 3 + $step) $dir 6 4))
                    if ($body.Height -lt 64) { throw "Missing Grace pose $dir $mode $step" }
                    $height = if ($step -eq 1) { 170 } else { 172 }
                    Save-Art $art $body "Actors/$($poses[$mode])/$($dir * 3 + $step)" 164 172 ([int]($height * $body.Width / [double]$body.Height)) $height -clean $true
                    if ($mode -eq 0 -and $step -eq 1) {
                        Save-Art $art $body "Actors/GraceIdleUnified/$dir" 164 172 ([int]($height * $body.Width / [double]$body.Height)) $height -clean $true
                    }
                }
            }
        }
    } finally { $art.Dispose() }

    $art = Open-Art '34079283-f1d5-4148-9b9a-035353f96b1c'
    $visitors = @('VisitorA', 'VisitorB')
    try {
        for ($dir = 0; $dir -lt 4; $dir++) {
            for ($visitor = 0; $visitor -lt 2; $visitor++) {
                $row = if ($visitor -eq 0) { $dir } else { @(0, 2, 1, 3)[$dir] }
                for ($step = 0; $step -lt 3; $step++) {
                    $body = [SpriteCrop]::LargestBody($art, (Cell $art ($visitor * 3 + $step) $row 6 4))
                    if ($body.Height -lt 64) { throw "Missing visitor pose $dir $visitor $step" }
                    $width = [int](208.0 * $body.Width / $body.Height)
                    Save-Art $art $body "Actors/$($visitors[$visitor])Walk/$($dir * 3 + $step)" 164 208 $width 208 -clean $true
                    if ($step -eq 1) { Save-Art $art $body "Actors/$($visitors[$visitor])/$dir" 164 208 $width 208 -clean $true }
                }
            }
        }
    } finally { $art.Dispose() }

    $portraits = @{
        Grace = @('Grace', 0); Mom = @('Mom', 1); Dad = @('Dad', 0); DadSad = @('Dad', 5)
        Grandma = @('Grandma', 0); Grandpa = @('Grandpa', 0); Uncle = @('UncleJames', 0); Aunt = @('Aunt', 0)
    }
    foreach ($portrait in $portraits.GetEnumerator()) {
        $art = [Drawing.Bitmap]::FromFile((Join-Path $old "img/faces/RM_Face_$($portrait.Value[0]).png"))
        try {
            $index = [int]$portrait.Value[1]
            Save-Art $art (Cell $art ($index % 4) ([Math]::Floor($index / 4)) 4 2) "Portraits/$($portrait.Key)" 144 144
        } finally { $art.Dispose() }
    }
    return
}

if ($OnlyRevision) {
    $revision = @{
        'Ground/LawnTexture' = @('05afdea8-e39d-4561-9fcd-b77098c2e87b', 1200, 1200)
        'Ground/PathStone' = @('dd3dc2de-a154-4410-86c6-7e91c81c6600', 768, 768)
        'Story/WindowView' = @('88104bb4-dacd-4c00-b67e-5ea8f8660882', 1232, 944)
    }
    foreach ($entry in $revision.GetEnumerator()) {
        $art = Open-Art $entry.Value[0]
        try { Save-Art $art ([Drawing.Rectangle]::new(0,0,$art.Width,$art.Height)) $entry.Key $entry.Value[1] $entry.Value[2] }
        finally { $art.Dispose() }
    }
    $art = Open-Art 'b3c5f03a-e48d-42fa-80a6-334e2f3c298a'
    $objects = @('ConsoleFront','DoorNew','DoorOpenNew','Daisies','Lilies','Puddle','Plaque','Register')
    try {
        for ($i = 0; $i -lt $objects.Length; $i++) {
            Place-Cell $art (Cell $art ($i % 4) ([Math]::Floor($i / 4)) 4 2) "Props/$($objects[$i])" 300
        }
    } finally { $art.Dispose() }

    foreach ($name in 'Aunt','Uncle') {
        $file = if ($name -eq 'Uncle') { '$RM_UncleJames.png' } else { '$RM_Aunt.png' }
        $art = [Drawing.Bitmap]::FromFile((Join-Path $old "img/characters/$file"))
        try {
            for ($i = 0; $i -lt 4; $i++) {
                $body = [SpriteCrop]::LargestBody($art, (Cell $art 1 $i 3 4))
                Save-Art $art $body "Actors/$name/$i" 164 208 ([int]($body.Width * 208.0 / $body.Height)) 208 -clean $true -sharpen $true
            }
        } finally { $art.Dispose() }
    }

    foreach ($name in 'GraceRun','GraceCrouch') {
        $out = Join-Path $destination "Actors/${name}Matched"
        New-Item -Force -ItemType Directory -Path $out | Out-Null
        $start = if ($name -eq 'GraceRun') { 80 } else { 92 }
        $lift = if ($name -eq 'GraceRun') { 13 } else { 17 }
        for ($i = 0; $i -lt 12; $i++) {
            $frame = [Drawing.Bitmap]::FromFile((Join-Path $destination "Actors/$name/$i.png"))
            try {
                [SpriteCrop]::MatchCoat($frame, $start, $lift)
                $frame.Save((Join-Path $out "$i.png"), [Drawing.Imaging.ImageFormat]::Png)
            } finally { $frame.Dispose() }
        }
    }

    if ($ModernMourners) {
        $art = Open-Art $ModernMourners
        try {
            for ($i = 0; $i -lt 4; $i++) {
                $body = [SpriteCrop]::LargestBody($art, (Cell $art $i 0 4 1))
                Save-Art $art $body "Actors/Mourners/$i" 164 208 ([int]($body.Width * 208.0 / $body.Height)) 208 -clean $true
            }
        } finally { $art.Dispose() }
    }
    return
}

$grounds = @{
    Cemetery='c1afabb8-7ce2-4233-a321-16bdf0fc7ff9'
    Flashback='3e8aad84-9786-4eb0-8efa-72ffa977f0d3'
    Hall='7395ab9f-a0ae-4250-aad8-d96500e84674'
    Study='8535e15e-4070-4035-b729-6d4d1e7f0041'
}
foreach ($name in $grounds.Keys) {
    $art = Open-Art $grounds[$name]
    try { Save-Art $art ([Drawing.Rectangle]::new(0,0,$art.Width,$art.Height)) "Ground/$name" 2304 1728 }
    finally { $art.Dispose() }
}

$objects=@('GraveMain','GraveMoss','GraveSlate','GraveWilt','GraveStones','Fence','Tree','Bench','Bag','Parcel',
    'Sofa','Sideboard','Cabinet','Chair','TeaTable')
$art=Open-Art 'b8a262f9-1f46-449f-9617-079711286416'
try {
    for($index=0;$index -lt $objects.Length;$index++){
        Place-Cell $art (Cell $art ($index % 5) ([int][Math]::Floor($index/5)) 5 3) "Props/$($objects[$index])" 300
    }
} finally { $art.Dispose() }
$art=Open-Art '4be19c42-fbd3-49b1-a226-de6d36f3b8ab'
$objects=@('Cup','Letters','Book','Sewing','Door','DoorOpen','Ribbon','Birdbath')
try {
    for($index=0;$index -lt $objects.Length;$index++){
        Place-Cell $art (Cell $art ($index % 4) ([int][Math]::Floor($index/4)) 4 2) "Props/$($objects[$index])" 225
    }
} finally { $art.Dispose() }
$art=Open-Art $RemainingProps
try {
    $width=$art.Width; $height=$art.Height
    Place-Cell $art ([Drawing.Rectangle]::new(10,0,[int]($width*.317),$height)) 'Props/Console' 310
    Place-Cell $art ([Drawing.Rectangle]::new([int]($width*.39),0,[int]($width*.17),$height)) 'Props/FenceSide' 270
    Place-Cell $art ([Drawing.Rectangle]::new([int]($width*.58),0,[int]($width*.42),$height)) 'Props/Rug' 640
} finally { $art.Dispose() }

$grace=Open-Art 'bec25245-1816-4066-a9f0-907a0ecf3974'
$inMotion=Open-Art '2dac06e6-dfeb-4be0-93ca-4ff6f93ec6c7'
try {
    for($dir=0;$dir -lt 4;$dir++){
        $idle=Find-Body $grace (Cell $grace $dir 0 4 4)
        Save-Art $grace $idle "Actors/Grace/$dir" 164 172 ([int]($idle.Width*172.0/$idle.Height)) 172 -clean $true
        foreach($pose in 'Walk','Run','Crouch'){
            for($frame=0;$frame -lt 3;$frame++){
                $index=$dir*3+$frame
                $sourceArt=$grace; $cell=Cell $grace $dir 1 4 4; $flip=$false
                $height=170
                if ($pose -eq 'Walk'){
                    if($frame -eq 1){$cell=Cell $grace $dir 0 4 4; $height=172}
                    if($frame -eq 2 -and $dir -in 0,3){$flip=$true}
                    if($frame -eq 2 -and $dir -in 1,2){
                        $sourceArt=$inMotion; $cell=Cell $inMotion 2 $dir 6 4
                    }
                } elseif($pose -eq 'Run') {
                    $sourceArt=$inMotion; $cell=Cell $inMotion $frame $dir 6 4
                    $height=if($frame -eq 1){151}else{173}
                } else {
                    $sourceArt=$inMotion; $cell=Cell $inMotion ($frame+3) $dir 6 4
                    $height=if($frame -eq 1){133}else{137}
                }
                $body=Find-Body $sourceArt $cell
                Save-Art $sourceArt $body "Actors/Grace$pose/$index" 164 172 ([int]($body.Width*$height/[double]$body.Height)) $height $flip $true
            }
        }
    }
} finally { $grace.Dispose(); $inMotion.Dispose() }

$art=Open-Art 'e99e7ff1-a8df-4a23-8784-6bd13ee5af6b'
$names=@('Dad','Mom','Grandma','Grandpa')
try {
    for($col=0;$col -lt $names.Length;$col++){
        for($dir=0;$dir -lt 4;$dir++){
            $row=@(0,1,1,2)[$dir]
            $body=Find-Body $art (Cell $art $col $row 4 3)
            Save-Art $art $body "Actors/$($names[$col])/$dir" 164 208 ([int]($body.Width*208.0/$body.Height)) 208 ($dir -eq 2) $true
        }
    }
} finally { $art.Dispose() }

$pictures=@{ Cover='titles1/RM_Title'; Chapter='pictures/RM_Act1Cover'; Gravestone='pictures/RM_Gravestone';
    ClosedParcel='pictures/RM_ParcelCloseup'; Mechanism='pictures/RM_MusicBoxStorybook'; Reunion='pictures/RM_MemoryChapter' }
foreach($key in $pictures.Keys){
    $art=if($Story.ContainsKey($key)){ Open-Art $Story[$key] }
        else { [Drawing.Bitmap]::FromFile((Join-Path $old "img/$($pictures[$key]).png")) }
    try { Save-Art $art ([Drawing.Rectangle]::new(0,0,$art.Width,$art.Height)) "Story/$key" 1232 944 }
    finally { $art.Dispose() }
}
$art=Open-Art $OpenParcel
try { Save-Art $art ([Drawing.Rectangle]::new(0,0,$art.Width,$art.Height)) 'Story/OpenParcel' 1232 944 }
finally { $art.Dispose() }

$audio=Join-Path $PSScriptRoot 'Assets/Audio'
New-Item -Force -ItemType Directory -Path $audio | Out-Null
foreach($name in 'RM_StoryTitle','RM_StoryFarewell','RM_StoryFlashback','RM_StoryKeepsake','RM_StoryRain',
    'RM_StoryRoom','RM_StoryStone','RM_StoryWood','RM_Cloth','RM_DoorCreak','RM_DoorPalm'){
    $sourceAudio=Get-ChildItem (Join-Path $old 'audio') -Recurse -Filter "$name.ogg" | Select-Object -First 1
    if(!$sourceAudio){throw "Missing audio $name"}
    Copy-Item -LiteralPath $sourceAudio.FullName -Destination $audio
}
