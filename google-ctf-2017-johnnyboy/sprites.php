<?php
// Arguments: <offset> <byte length> <pixels per row>
$offset = $argv[1];
$length = $argv[2];
$pprow = $argv[3];

$bin = file_get_contents('chall.ino.bin');
$img = imagecreatetruecolor($pprow, ceil(($length * 8) / $pprow));
$white = imagecolorallocate($img, 255, 255, 255);

for ($i = 0; $i < $length; $i++) {
    $byte = unpack('c', substr($bin, $offset + $i, 1))[1];
    for ($j = 0; $j < 8; $j++) {
        if ($byte & (1 << $j)) {
            $x = (int)((($i * 8) + (8 - $j)) % $pprow);
            $y = (int)((($i * 8) + (8 - $j)) / $pprow);
            imagesetpixel($img, $x, $y, $white);
        }
    }
}
imagepng($img, 'out.png');