.NET Core ile Microservice Choreography-Based Saga Pattern kodlamas� yap�lm��t�r.

- Distributed Transaction nedir ?

Distributed Transaction senaryolar�nda, microservice'ler aras�nda data consistency(veri tutarl���) olay�n� y�netmeyi imkan veren bir pattern'd�r. Her bir microservice baz� durumlarda shared bir veritaban�na ba�lanabilir. Ama bizim genellikle istedi�imiz her microservicein kendisine ait bir veritaban� olmas�. Birden fazla microservice i�eren sistemde �rne�in sipari�in olu�turulmas� sto�un d��mesi gibi her biri ayr� microservice de ger�ekle�iyorsa biz bu durumlarda transactionlar� y�netmemiz laz�m.

- ACID Nedir?

De�i�ikliklerin veritaban�na nas�l uygulanaca��n� belirten prensiplerdir. Transactionlar'�n ACID (atomicity, consistency,isolation,durability) olmal�d�r.

	Atomicity: Ya hep, ya hi�
	Consistency: Datalar�n tutarl� olmas�. Veritaban�n� s�rekli valid tutar.
	Isolation: Transactionlar�n birbirinden ba��ms�z olmas�n� ifade eder.
	Durability: Datalar�n g�venli bir ortamda saklanmas�n� ifade eder.

- Choreography-based saga

Local transaction s�ras�n� kullanarak bir transaction y�netimi sa�lar.

	Uygulamas� daha kolayd�r. 
	distributed transaction'a kat�lacak 2 ile 4 microservice aras�nda bir distributed transaction y�netimi i�in uygun bir implementasyon'dur.
	Sisteme kat�lan her bir kat�l�mc� karar vericidir. (ba�ar�l� veya ba�ar�s�z)
	Choreography implement etmenin bir yolu asynchronous messaging pattern kullanmakt�r.
	Her servis kuyru�u dinler, gelen event/message ile ilgili i�lemi yapar, sonu� olarak ba�ar�l� veya ba�ar�s�z durumunu tekrar kuyru�a d�ner
	Point-to-point bir ileti�im olmad���ndan serviceler aras� coupling azal�r.
	Transaction y�netimi merkezi olmad��� i�in performance bottlenect azal�r.
	Compensable transaction : Bir transaction'lar�n yapm�� oldu�u i�lemi tersine alan transaction'lard�r. (z�t transaction)