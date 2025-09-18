/* --- 簡易購物車（localStorage）--- */
const CART_KEY = "mc_cart";

function readCart() {
  try { return JSON.parse(localStorage.getItem(CART_KEY) || "[]"); } catch { return []; }
}
function saveCart(items) {
  localStorage.setItem(CART_KEY, JSON.stringify(items));
}
function addToCart(product) {
  const cart = readCart();
  const idx = cart.findIndex(i => i.id === product.id);
  if (idx >= 0) {
    cart[idx].qty += product.qty || 1;
  } else {
    cart.push({ id: product.id, name: product.name, price: product.price, qty: product.qty || 1, imageUrl: product.imageUrl || "" });
  }
  saveCart(cart);
  alert("已加入購物車");
}
function removeFromCart(id) {
  const cart = readCart().filter(i => i.id !== id);
  saveCart(cart);
  renderCart && renderCart();
}
function updateQty(id, qty) {
  const cart = readCart();
  const item = cart.find(i => i.id === id);
  if (!item) return;
  item.qty = Math.max(1, parseInt(qty || "1", 10));
  saveCart(cart);
  renderCart && renderCart();
}
function cartTotal() {
  return readCart().reduce((sum, i) => sum + i.price * i.qty, 0);
}

/* --- 假資料（實務上請從後端API取） --- */
const demoProducts = [
  { id: 1, name: "經典T-Shirt", sku: "TS-001", price: 399, qty: 20, imageUrl: "https://picsum.photos/seed/t1/600/400", description: "舒適純棉，日常必備。" },
  { id: 2, name: "商務襯衫", sku: "SH-002", price: 899, qty: 12, imageUrl: "https://picsum.photos/seed/t2/600/400", description: "合身剪裁，正式/休閒皆宜。" },
  { id: 3, name: "休閒短褲", sku: "PT-003", price: 599, qty: 8, imageUrl: "https://picsum.photos/seed/t3/600/400", description: "輕薄透氣，夏日首選。" }
];

/* --- Storefront render --- */
function renderStorefront() {
  const el = document.getElementById("store-grid");
  if (!el) return;
  const html = demoProducts.map(p => `
    <div class="col">
      <div class="card h-100 shadow-sm">
        <img src="${p.imageUrl}" class="card-img-top object-cover" alt="${p.name}" height="180">
        <div class="card-body d-flex flex-column">
          <h5 class="card-title">${p.name}</h5>
          <div class="mb-2"><span class="badge text-bg-secondary badge-sku">SKU ${p.sku}</span></div>
          <p class="card-text small text-secondary flex-grow-1">${p.description}</p>
          <div class="d-flex align-items-center justify-content-between">
            <div class="fs-5">\$${p.price}</div>
            <button class="btn btn-primary" onclick='addToCart({id:${p.id}, name:"${p.name}", price:${p.price}, imageUrl:"${p.imageUrl}"})'>明細</button>
          </div>
        </div>
      </div>
    </div>
  `).join("");
  el.innerHTML = html;
}

/* --- Cart page render --- */
function renderCart() {
  const tbody = document.getElementById("cart-body");
  const totalEl = document.getElementById("cart-total");
  if (!tbody || !totalEl) return;
  const cart = readCart();
  if (cart.length === 0) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-center text-secondary py-4">購物車目前是空的</td></tr>`;
    totalEl.textContent = "0";
    return;
  }
  tbody.innerHTML = cart.map(i => `
    <tr>
      <td width="64"><img src="${i.imageUrl || "https://picsum.photos/seed/na/100/100"}" class="rounded object-cover" width="64" height="64"></td>
      <td>${i.name}</td>
      <td>\$${i.price}</td>
      <td width="140">
        <input type="number" class="form-control" min="1" value="${i.qty}" onchange="updateQty(${i.id}, this.value)" />
      </td>
      <td class="text-end">
        <button class="btn btn-outline-danger btn-sm" onclick="removeFromCart(${i.id})">移除</button>
      </td>
    </tr>
  `).join("");
  totalEl.textContent = cartTotal().toFixed(0);
}

/* --- Checkout --- */
async function handleCheckoutSubmit(e) {
  e.preventDefault();
  const cart = readCart();
  if (cart.length === 0) {
    alert("購物車為空");
    return;
  }
  const form = e.target;
  const data = Object.fromEntries(new FormData(form).entries());
  const payload = {
    customer: data,
    items: cart.map(i => ({ productId: i.id, qty: i.qty, unitPrice: i.price })),
    total: cartTotal()
  };

  /* 實務上：POST 到後端（.NET Core MVC / API）
  const res = await fetch('/Orders/Create', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
    body: JSON.stringify(payload)
  });
  const result = await res.json();
  if (!res.ok) { alert(result.message || '下單失敗'); return; }
  localStorage.removeItem(CART_KEY);
  window.location.href = `order-success.html?no=${encodeURIComponent(result.orderNo)}`;
  */

  // Demo：直接清空並跳轉
  localStorage.removeItem(CART_KEY);
  const demoOrderNo = "ORD" + Date.now();
  window.location.href = `order-success.html?no=${encodeURIComponent(demoOrderNo)}`;
}

function initCheckout() {
  const form = document.getElementById("checkout-form");
  if (form) form.addEventListener("submit", handleCheckoutSubmit);
  const totalEl = document.getElementById("checkout-total");
  if (totalEl) totalEl.textContent = cartTotal().toFixed(0);
}


/* --- 工具：若採用MVC Razor，可改用Tag Helpers並加入 Anti-Forgery Token --- */
function getAntiForgeryToken() {
  const el = document.querySelector('input[name="__RequestVerificationToken"]');
  return el ? el.value : "";
}

function SumDatareduce(arr) {
    if (arr.length == 0) return 0;
    return arr.reduce((a, b) => a + b);
}

/* --- 初始化：根據頁面呼叫對應渲染 --- */
document.addEventListener("DOMContentLoaded", () => {
  renderCart();
  initCheckout();
});