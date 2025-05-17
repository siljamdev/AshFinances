class Day{
	public float start{get; set{
		field = value;
		update();
	}}
	
	public float end{get; private set;}
	
	public List<Transaction> transactions{get; private set;}
	
	public Day(float s, List<Transaction> l){		
		transactions = l;
		
		start = s;
	}
	
	public Day(float s) : this(s, new List<Transaction>()){}
	
	void update(){
		end = start + transactions.Sum(t => t.number);
	}
	
	public void addTransaction(Transaction t){
		transactions.Add(t);
		update();
	}
	
	public void deleteTransaction(Transaction t){
		transactions.Remove(t);
		update();
	}
}