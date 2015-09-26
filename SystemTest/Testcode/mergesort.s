 .text
 
# text of list processing:
main: 
    la   $s7,mem     	# Load 'mem' address into $s7
		add  $s6, $s7, $zero

loop:
		lw 	 $t0, ($s6)
		lw   $t1,4($s6) 
		add  $s5, $s6, $zero
		bltz $t1, finish
		bgt  $t0, $t1, moveList
		addi $s6, $s6, 4
		j loop

finishMoveList:
		sw   $t1,($s5)
		addi $s6, $s6, 4
		j		 loop

moveList:
	sw	 $t0,4($s5)
	beq  $s5, $s7, finishMoveList
	addi $s5, $s5, -4
	lw	 $t0,($s5) 
	bgt  $t1, $t0, insert
	j		 moveList

insert:
	sw	 $t1, 4($s5) 
	addi $s6, $s6, 4
	j		 loop

finish:
	li   $v0,10       	# Load up 'exit' syscall
  syscall           	# Terminate application

.data
# Our memory list is a series of words, terminated with -1
mem: .word 7, 3, 2, 15, 4, 6, 5, 13, 1, 12, 11, 14, 10, 9, 8, -1