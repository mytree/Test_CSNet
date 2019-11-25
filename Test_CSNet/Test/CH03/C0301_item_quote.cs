using System;               // String, Boolean

namespace Test_CSNet {
    public class ItemQuote {
        public long itemNumber;             // 상품 식별 번호
        public String itemDescription;      // 상품 설명
        public int quantity;                // 단가에 포함된 상품 개수(언제나 1이상이어야 한다.)
        public int unitPrice;               // 상품 단가(단위: 센트)
        public Boolean discounted;          // 할인 여부
        public Boolean inStock;             // 운송 가능한 재고 존재 여부

        public ItemQuote() {}
        public ItemQuote(long itemNumber, String itemDescription, 
            int quantity, int unitPrice, Boolean discounted, Boolean inStock) {
            this.itemNumber = itemNumber;
            this.itemDescription = itemDescription;
            this.quantity = quantity;
            this.unitPrice = unitPrice;
            this.discounted = discounted;
            this.inStock = inStock;
        }
        public override string ToString() {
            String EOLN = "\n";
            String value = "Item# = " + itemNumber + EOLN +
                "Description = " + itemDescription + EOLN +
                "Quantity = " + quantity + EOLN +
                "Price (each) = " + unitPrice + EOLN +
                "Total Price = " + (quantity + unitPrice);
            if (discounted) {
                value += " (discounted)";
            }
            if (inStock) value += EOLN + "In Stock" + EOLN;
            else value += EOLN + "Out of Stock" + EOLN;
            return value;
        }

    }
}
