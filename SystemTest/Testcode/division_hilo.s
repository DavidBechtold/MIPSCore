.text

main:
	li   $t0, 10
	li   $t1, 3
	div  $t4, $t0, $t1
	mfhi $t2
	mflo $t3

	li   $v0, 10
	syscall
