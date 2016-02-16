# David Bechtold 
 
.text
 
# text of list processing:
main: 
	li   $a0, 4

	addi $sp, $sp, -8
	sw   $a0, 0($sp)				# Argument for the factorial function
	jal  factorial
	lw   $t5, 4($sp)				# save return value to $t5
	addi $sp, $sp, 8
	li   $v0,10       			# Load up 'exit' syscall
  syscall           			# Terminate application

factorial:
	# stack: ReturnValue | Argument | $ra | $t0 | $t1
	sw   $ra, -4($sp)
	sw   $t0, -8($sp)
	sw   $t1, -12($sp)
	lw   $t0,  0($sp)					# Load function argument
	lw   $t1,  4($sp)					# Load function return 
	addi $sp, $sp, -12

	li   $t1, 1
	beq  $t1, $t0, return			# Abort recursion criteria

  addi $t1, $t0, -1
	addi $sp, $sp, -8
	sw   $t1, 0($sp)					# Argument for the factorial function
	jal  factorial

	lw   $t1, 4($sp)
	addi $sp, $sp, 8

	multu $t0, $t1
	mflo  $t1

return:
	sw   $t1, 16($sp)
	sw   $t0, 12($sp)
	lw   $ra, 8($sp)
	lw   $t0, 4($sp)
	lw   $t1, 0($sp)
	addi $sp, $sp, 12
	jr $ra