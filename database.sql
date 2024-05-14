CREATE DATABASE ecommerce;

--uuid-ossp
SELECT
    *
FROM
    pg_extension
WHERE
    extname = 'uuid-ossp';

CREATE EXTENSION "uuid-ossp";

--EMTINAN
--Create Categories Table
CREATE TABLE
    categories (
        category_id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
        category_name VARCHAR(100) NOT NULL,
        category_slug VARCHAR(100) UNIQUE NOT NULL,
        category_description TEXT
    );

--Insert Categories
INSERT INTO
    categories (
        category_name,
        category_slug,
        category_description
    )
VALUES
    (
        'Computer & Tablets',
        'computer-&-tablets',
        'This category includes a wide range of devices, including desktop computers, laptops, tablets, and accessories such as keyboards, mice, and styluses. Customers can find products related to computing and mobile productivity in this category.'
    ),
    (
        'Computer Supplies',
        'computer-supplies',
        'This category focuses on essential supplies and peripherals for computers, including cables, adapters, storage devices (such as USB flash drives and external hard drives), printer supplies (such as ink cartridges and toner), and other accessories needed to enhance or maintain computer systems.'
    ),
    (
        'Smartphones',
        'smartphones',
        'This category features a variety of smartphones from different manufacturers, including popular brands like Apple, Samsung, Google, and others. Customers can browse through the latest models, compare features, and find the perfect smartphone to meet their needs.'
    ),
    (
        'Smartwatches',
        'smartwatches',
        'Smartwatches have become increasingly popular as wearable technology, offering features such as fitness tracking, notifications, and access to apps. This category includes a selection of smartwatches from various brands, providing customers with options to stay connected and track their activities conveniently.'
    ),
    (
        'Smartphones Accessories',
        'smartphones-accessories',
        'This category complements the smartphones category by offering a range of accessories to enhance the functionality and usability of smartphones. Customers can find items such as cases, screen protectors, chargers, cables, wireless headphones, and other accessories to personalize and protect their smartphones.'
    );

--Read Categories
SELECT
    *
FROM
    categories;

--DROP TABLE Categories;
--ENAS && nouir
--Create user Table
CREATE TABLE
    users (
        user_id UUID UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
        user_name VARCHAR(100) NOT NULL,
        user_email varchar(100) UNIQUE NOT NULL,
        user_password varchar(255) NOT NULL,
        user_address varchar(100) NOT NULL,
        user_image VARCHAR(100),
        isAdmin BOOLEAN DEFAULT FALSE,
        isBanned BOOLEAN DEFAULT FALSE,
        createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );

--Insert USER
INSERT INTO
    users (
        user_name,
        user_email,
        user_password,
        user_address,
        user_image,
        isAdmin,
        isBanned
    )
VALUES
    (
        'Yusef_Ahmed',
        'Y_Ahmed@gmail.com',
        '12345',
        'Egypt',
        'profile.jpg',
        FALSE,
        FALSE
    ),
    (
        'Mai_Ali',
        'maiali@gmail.com',
        '857302',
        'KSA',
        'NULL',
        FALSE,
        FALSE
    ),
    (
        'Fatimah_Mohamed',
        'fafmoh@gmail.com',
        '76321',
        'KSA',
        'NULL',
        TRUE,
        FALSE
    ),
    (
        'Enas_Kutbi',
        'enaskutbi@gmail.com',
        '0029837',
        'KSA',
        'USER.JPG',
        TRUE,
        TRUE
    ),
    (
        'Emtinan_Maji',
        'emtinanmaji@gmail.com',
        '5476980',
        'KSA',
        'profile.jpg',
        TRUE,
        TRUE
    ),
    (
        'nouir Alosaimi',
        'nouir@gmail.com',
        '8888',
        'KSA',
        'profile.jpg',
        TRUE,
        TRUE
    );

--Update a  name column
UPDATE users
SET
    user_name = 'Fatima'
WHERE
    user_email = 'fafmoh@gmail.com';

--Read USER Table
SELECT
    *
FROM
    users;

--Delete from user
DELETE FROM users
WHERE
    user_address != 'KSA';

--ATHEER 
-- Orders Table
CREATE TABLE
    orders (
        order_id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
        order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        order_status VARCHAR(50) DEFAULT 'Pending',
        user_id uuid,
        CONSTRAINT fk_users FOREIGN KEY (user_id) REFERENCES users (user_id),
        --product_id uuid, CONSTRAINT fk_products FOREIGN KEY(product_id) REFERENCES products(product_id),
        order_total INT NOT NULL
    );

--Insert Orders
INSERT INTO
    orders (order_status, order_total)
VALUES
    ('Processing', 63),
    ('Canceled', 222),
    ('Pending', 57),
    ('Pending', 88),
    ('Processing', 92);

-- Read Orders Table
SELECT
    *
FROM
    orders;

-- JOIN
SELECT
    order_date,
    order_status,
    order_total,
    user_name,
    user_email,
    user_address
FROM
    orders
    INNER JOIN users ON orders.user_id = users.user_id;

-- Order_Item Table
CREATE TABLE
    order_item (
        order_item UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
        quantity INT,
        order_id UUID,
        CONSTRAINT fk_orders FOREIGN KEY (order_id) REFERENCES orders (order_id),
        product_id UUID,
        CONSTRAINT fk_products FOREIGN KEY (product_id) REFERENCES products (product_id)
    );

-- Insert order_item
INSERT INTO
    order_item (quantity, order_id)
VALUES
    (2, 1);

(5, 2);

(1, 3);

(9, 4);

(12, 5);

-- JOIN order_item & orders
SELECT
    order_date,
    order_total,
    order_status,
    quantity
FROM
    order_item
    INNER JOIN orders ON order_item.order_id = orders.order_id;

-- JOIN order_item & products
SELECT
    product_id,
    product_name,
    product_price,
    quantity
FROM
    order_item
    INNER JOIN products ON order_item.product_id = products.product_id;

--nouir && Enas
--create product table
CREATE TABLE
    products (
        product_id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
        product_name varchar(50) NOT NULL,
        product_slug VARCHAR(100) UNIQUE NOT NULL,
        product_image VARCHAR(100),
        product_description TEXT NOT NULL,
        product_price DECIMAL(10, 2) NOT NULL,
        product_quantity INT DEFAULT 0,
        product_sold INT DEFAULT 0,
        shipping DECIMAL(10, 2) DEFAULT 0,
        category_id UUID,
        CONSTRAINT fk_categories FOREIGN KEY (category_id) REFERENCES categories (category_id),
        createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );

--insert product
INSERT INTO
    products (
        product_name,
        product_slug,
        product_image,
        product_description,
        product_price,
        product_quantity,
        product_sold,
        shipping,
        createdAt
    )
VALUES
    (
        'iphone14 pro max',
        'iphone14-pro-max',
        'iphone14.png',
        '128GB BLACK COLOR 6.1 INCHES',
        3000.99,
        30,
        5,
        3,
        NOW ()
    ),
    (
        'iphone watch',
        'iphone-watch',
        'iphone_watch.png',
        'New Apple Watch SE (2nd Gen, 2023) [GPS + Cellular 40mm] Smartwatch with Starlight Aluminum Case with Starlight Sport Band S/M. Fitness & Sleep Tracker',
        1049.00,
        50,
        10,
        1,
        NOW ()
    ),
    (
        'usb-c to Lightning Adapter',
        'usb-c-to-Lightning-Adapter',
        'usb-ctoLightningAdapter.JPG',
        'The Apple 20W USB‑C Power Adapter offers fast, efficient charging at home, in the office, or on the go. Pair it with iPhone 8 or later for fast charging — 50 percent battery in around 30 minutes.¹ Or pair it with the iPad Pro and iPad Air for optimal charging performance. Compatible with any USB-C enabled device.',
        29,
        100,
        10,
        2,
        NOW ()
    ),
    (
        'iphone charger',
        'iphone-charger',
        'iphone_charger.png',
        'The Apple 20W USB‑C Power Adapter offers fast, efficient charging at home, in the office, or on the go. Pair it with iPhone 8 or later for fast charging — 50 percent battery in around 30 minutes.¹ Or pair it with the iPad Pro and iPad Air for optimal charging performance. Compatible with any USB-C enabled device.',
        39,
        40,
        5,
        4,
        NOW ()
    ),
    (
        'Elevation Lab GoStand Adjustable Stand for iphone',
        'Elevation-Lab-GoStand-Adjustable-Stand-for-iphone',
        'Elevation.JPG',
        'ElevationLab GoStand is the adjustable, lightweight stand for iPhone. The clever design lets you position your screen at a wide range of angles and folds flat to fit in your pocket. It features premium composite and silicone construction with a steel hinge. Precision indexable back support adjusts with a low-profile button. Perfect for streaming Apple Fitness+ workouts, travel, group video, watching movies, gaming, and more',
        75.95,
        66,
        12,
        20,
        NOW ()
    ),
    (
        'Mophie powerstation',
        'Mophie-powerstation',
        'Mophie.PNG',
        'The mophie powerstation provides fast, portable power with dual USB-C PD ports. That means you can charge two devices at once. Get up to 43 extra hours of power.* Tuck the compact powerstation in your bag, and you are ready for any power demands the day throws at you',
        300,
        40,
        2,
        0,
        NOW ()
    );

--READ 
select
    P.product_id,
    P.product_name,
    C.category_name
from
    products P
    inner join categories C ON P.category_id = C.category_id;