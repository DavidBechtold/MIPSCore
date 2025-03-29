/* Script for -z combreloc: combine and sort reloc sections */
OUTPUT_ARCH(mips)
ENTRY(_start)
SEARCH_DIR("/usr/local/mips-elf/lib");

SECTIONS
{
  .text 0x0000 : { *(.text) _etext = . ; }
  .data 0x0000 :
    AT ( ADDR (.text) + SIZEOF (.text) )
    { _data = . ; *(.data); *(.rodata); *(.sdata); _edata = . ;  }
  .reginfo 0x0000 :
	AT ( ADDR (.text) + SIZEOF (.text) + SIZEOF (.data) )
	{ _reginfo = . ; *(.reginfo); }
}
