#
# Makefile for the MIPSCore testprograms
#
# Targets:
#   all         generates object dumps off all testprogramms
#

FILENAMES = bubblesort.objdump	quicksort.objdump
OBJECTS   = bubblesort.o quicksort.o

CP					= mips-elf-gcc
CPFLAGS 		= -c

LD          = mips-elf-ld
LDFLAGS     = -T MIPSCore.ld


all: $(FILENAMES)

%.objdump: %.out
	mips-elf-objdump -D $< > $@

%.out: %.o
	mips-elf-ld -T MIPSCore.ld -o $@ $<
	
%.o: %.c
	mips-elf-gcc -c $<
	

clean:
	rm -f $(FILENAME).elf $(OBJECTS)


