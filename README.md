.NET Core ile Microservice Choreography-Based Saga Pattern kodlamasý yapýlmýþtýr.

- Distributed Transaction nedir ?

Distributed Transaction senaryolarýnda, microservice'ler arasýnda data consistency(veri tutarlýðý) olayýný yönetmeyi imkan veren bir pattern'dýr. Her bir microservice bazý durumlarda shared bir veritabanýna baðlanabilir. Ama bizim genellikle istediðimiz her microservicein kendisine ait bir veritabaný olmasý. Birden fazla microservice içeren sistemde örneðin sipariþin oluþturulmasý stoðun düþmesi gibi her biri ayrý microservice de gerçekleþiyorsa biz bu durumlarda transactionlarý yönetmemiz lazým.

- ACID Nedir?

Deðiþikliklerin veritabanýna nasýl uygulanacaðýný belirten prensiplerdir. Transactionlar'ýn ACID (atomicity, consistency,isolation,durability) olmalýdýr.

	Atomicity: Ya hep, ya hiç
	Consistency: Datalarýn tutarlý olmasý. Veritabanýný sürekli valid tutar.
	Isolation: Transactionlarýn birbirinden baðýmsýz olmasýný ifade eder.
	Durability: Datalarýn güvenli bir ortamda saklanmasýný ifade eder.

- Choreography-based saga

Local transaction sýrasýný kullanarak bir transaction yönetimi saðlar.

	Uygulamasý daha kolaydýr. 
	distributed transaction'a katýlacak 2 ile 4 microservice arasýnda bir distributed transaction yönetimi için uygun bir implementasyon'dur.
	Sisteme katýlan her bir katýlýmcý karar vericidir. (baþarýlý veya baþarýsýz)
	Choreography implement etmenin bir yolu asynchronous messaging pattern kullanmaktýr.
	Her servis kuyruðu dinler, gelen event/message ile ilgili iþlemi yapar, sonuç olarak baþarýlý veya baþarýsýz durumunu tekrar kuyruða döner
	Point-to-point bir iletiþim olmadýðýndan serviceler arasý coupling azalýr.
	Transaction yönetimi merkezi olmadýðý için performance bottlenect azalýr.
	Compensable transaction : Bir transaction'larýn yapmýþ olduðu iþlemi tersine alan transaction'lardýr. (zýt transaction)