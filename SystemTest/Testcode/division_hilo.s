.text

main:
	li   $t0, 10
	li   $t1, 3
	div  $t0, $t1
	mfhi $t2
	mflo $t3
	move $t4, $t3

	li   $v0, 10
	syscall
